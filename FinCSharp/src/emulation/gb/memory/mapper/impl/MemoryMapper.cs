using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using fin.assert;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fin.emulation.gb.memory.mapper.impl {
  public sealed partial class MemorySourceFactory : IMemorySourceFactory {
    public IMemoryMapperBuilder BuildMapper(int size)
      => new MemoryMapperBuilder(size);

    private sealed class MemoryMapperBuilder : IMemoryMapperBuilder {
      private readonly MemoryMapper impl_;

      public MemoryMapperBuilder(int size) {
        this.impl_ = new MemoryMapper(size);
      }

      public IMemoryMapperBuilder Register(
          ushort address,
          IMemorySource source) {
        this.impl_.Register(address, source);
        return this;
      }

      public IMemoryMapper Build() {
        this.impl_.FinishAndFillGaps();
        return this.impl_;
      }
    }

    private sealed class MemoryMapper : IMemoryMapper {
      private class MemoryMapperSource : IMemorySource {
        private readonly IMemorySource impl_;

        public MemoryMapperSource(ushort address, IMemorySource impl) {
          this.Address = address;
          this.impl_ = impl;
        }

        public byte this[ushort address] {
          get => this.impl_[address];
          set => this.impl_[address] = value;
        }

        public int Size => this.impl_.Size;
        public ushort Address { get; }
      }

      private class AddressComparer : IComparer<ushort> {
        public int Compare([AllowNull] ushort lhs, [AllowNull] ushort rhs)
          => AddressComparer.Compare_(lhs, rhs);

        private static int Compare_(ushort? lhs, ushort? rhs)
          => (lhs ?? 0) - (rhs ?? 0);
      }

      private readonly SortedList<ushort, MemoryMapperSource> sourcesBuilder_ =
          new SortedList<ushort, MemoryMapperSource>(new AddressComparer());

      // TODO: Any equally fast way of doing this without this???
      private MemoryMapperSource[] sources_;
      private MemoryMapperSource[] sourcesMap_;

      public MemoryMapper(int size) {
        this.Size = size;
      }

      // TODO: Switch to builder pattern.
      public void Register(ushort address, IMemorySource source) {
        Asserts.True(source.Size > 0,
                     $"Source at {ByteFormatter.ToHex16(address)} is empty!");
        Asserts.True(address + source.Size <= this.Size,
                     $"Source at {ByteFormatter.ToHex16(address)} hangs off right end of mapper!");
        Asserts.True(address + source.Size <= this.Size,
                     $"Source at {ByteFormatter.ToHex16(address)} hangs off right end of mapper!");

        var newStartAddress = address;
        var newEndAddress = newStartAddress + (source.Size - 1);
        foreach (var existingKvp in this.sourcesBuilder_) {
          var existingStartAddress = existingKvp.Key;
          var existingEndAddress =
              existingStartAddress + (existingKvp.Value.Size - 1);
          if (newStartAddress <= existingEndAddress &&
              newEndAddress >= existingStartAddress) {
            Assert.Fail(
                $"New source at {ByteFormatter.ToHex16(address)} intersects with old source!");
          }
        }

        this.sourcesBuilder_.Add(address,
                                 new MemoryMapperSource(address, source));
      }

      public void FinishAndFillGaps() {
        MemoryMapperSource? prevSource = null;
        var originalSources = this.sourcesBuilder_.ToArray();
        foreach (var source in originalSources) {
          this.FillGapAfterIfPossible_(prevSource, source.Key);
          prevSource = source.Value;
        }
        this.FillGapAfterIfPossible_(prevSource, this.Size);

        // TODO: Blegh
        this.sources_ = this.sourcesBuilder_.Values.ToArray();
        this.sourcesMap_ = new MemoryMapperSource[this.Size];
        foreach (var sourceKvp in this.sourcesBuilder_) {
          var source = sourceKvp.Value;
          var sourceAddress = source.Address;
          var sourceSize = source.Size;
          for (int i = sourceAddress; i < sourceAddress + sourceSize; ++i) {
            this.sourcesMap_[i] = source;
          }
        }
      }

      private void FillGapAfterIfPossible_(
          MemoryMapperSource? previous,
          int nextAddress) {
        var previousAddress = previous?.Address ?? 0;
        var previousSize = previous?.Size ?? 0;

        var nextOpenAddress = (ushort) (previousAddress + previousSize);
        var remainingSize = (ushort) (nextAddress - nextOpenAddress);
        if (remainingSize > 0) {
          this.Register(nextOpenAddress, new MemoryArray(remainingSize));
        }
      }

      public byte this[ushort address] {
        get {
          var source = this.At_(address);
          if (source == null) {
            Asserts.Fail(
                $"Expected source at {ByteFormatter.ToHex16(address)} to be nonnull!");
          }
          var relativeAddress = (ushort) (address - source!.Address);
          return source[relativeAddress];
        }

        set {
          var source = this.At_(address);
          if (source == null) {
            Asserts.Fail(
                $"Expected source at {ByteFormatter.ToHex16(address)} to be nonnull!");
          }
          var relativeAddress = (ushort) (address - source!.Address);
          source[relativeAddress] = value;
        }
      }

      public int Size { get; }
      public int SourceCount => this.sources_.Length;

      // TODO: A better way of doing this??
      private MemoryMapperSource At_(ushort address) 
        => this.sourcesMap_[address];
    }
  }
}