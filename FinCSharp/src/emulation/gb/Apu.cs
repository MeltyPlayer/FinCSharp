using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace fin.emulation.gb {
  public class Apu {
    public const int NRX1_MAX_LENGTH = 64;
    public const int NR31_MAX_LENGTH = 256;
    public const int SWEEP_MAX_PERIOD = 8;
    public const int SOUND_MAX_FREQUENCY = 2047;
    public const int WAVE_SAMPLE_COUNT = 32;
    public const int NOISE_MAX_CLOCK_SHIFT = 13;
    public const int NOISE_DIVISOR_COUNT = 8;
    public const int ENVELOPE_MAX_PERIOD = 8;
    public const int ENVELOPE_MAX_VOLUME = 15;
    public const int DUTY_CYCLE_COUNT = 8;
    public const int SOUND_OUTPUT_MAX_VOLUME = 7;
    public const int WAVE_RAM_SIZE = 16;
    public const uint APU_TICKS_PER_SECOND = 2097152;

    public const int APU_CHANNEL_COUNT = 4;
    public const int SOUND_OUTPUT_COUNT = 2;
    public const int AUDIO_BUFFER_EXTRA_FRAMES = 256;

    public const int APU_TICKS = 2;
    public const int FREQUENCY = 44100;

    public const int BUFFER_SIZE =
        (2048 + AUDIO_BUFFER_EXTRA_FRAMES) * SOUND_OUTPUT_COUNT;

    public const int FRAME_SEQUENCER_COUNT = 8;
    public const int FRAME_SEQUENCER_TICKS = 8192; // 512Hz
    public const int FRAME_SEQUENCER_UPDATE_ENVELOPE_FRAME = 7;

    public enum SweepDirection {
      SWEEP_DIRECTION_ADDITION = 0,
      SWEEP_DIRECTION_SUBTRACTION = 1,
    }

    public enum EnvelopeDirection {
      ENVELOPE_ATTENUATE = 0,
      ENVELOPE_AMPLIFY = 1,
    }

    public enum WaveDuty {
      WAVE_DUTY_12_5 = 0,
      WAVE_DUTY_25 = 1,
      WAVE_DUTY_50 = 2,
      WAVE_DUTY_75 = 3,
    }

    public const int WAVE_DUTY_COUNT = 4;

    public enum WaveVolume {
      WAVE_VOLUME_MUTE = 0,
      WAVE_VOLUME_100 = 1,
      WAVE_VOLUME_50 = 2,
      WAVE_VOLUME_25 = 3,
    }

    public const int WAVE_VOLUME_COUNT = 4;

    public enum Sound {
      SOUND1 = 0,
      SOUND2 = 1,
      SOUND3 = 2,
      SOUND4 = 3,
      VIN = 4,
    };

    public const int SOUND_COUNT = 5;

    public static byte[] DUTY = {
        // WAVE_DUTY_12_5
        0, 0, 0, 0, 0, 0, 0, 1,
        // WAVE_DUTY_25
        1, 0, 0, 0, 0, 0, 0, 1,
        // WAVE_DUTY_50
        1, 0, 0, 0, 0, 1, 1, 1,
        // WAVE_DUTY_75
        0, 1, 1, 1, 1, 1, 1, 0,
    };

    public static byte GetDuty(WaveDuty waveDuty, byte dutyIndex)
      => Apu.DUTY[(int) waveDuty * Apu.WAVE_DUTY_COUNT + dutyIndex];

    private readonly uint[] so_volume = new uint[SOUND_OUTPUT_COUNT];

    private readonly bool[] so_output =
        new bool[SOUND_COUNT * SOUND_OUTPUT_COUNT];

    public bool GetSoOutput(Sound sound, int output)
      => this.GetSoOutput((int) sound, output);

    public bool GetSoOutput(int sound, int output)
      => this.so_output[sound * Apu.SOUND_COUNT + output];

    public void SetSoOutput(Sound sound, int output, bool value)
      => this.so_output[(int) sound * Apu.SOUND_COUNT + output] = value;

    private readonly bool enabled = true;
    private readonly Sweep sweep;
    private readonly Wave wave;
    private readonly Noise noise;
    private readonly Channel[] channel = new Channel[APU_CHANNEL_COUNT];
    private byte frame_;
    private int syncTicks_;

    private Channel Channel1 => this.channel[0];
    private Channel Channel2 => this.channel[1];
    private Channel Channel3 => this.channel[2];
    private Channel Channel4 => this.channel[3];

    private static readonly byte[] s_divisors = {
        8, 16, 32, 48, 64, 80, 96, 112
    };


    private uint divisor_;
    private uint freqCounter_;

    public void Tick(int cyclesThisIteration) {
      /*if (this.enabled) {
        this.Update_(cyclesThisIteration);
      } else {
        for (var i = cyclesThisIteration; i > 0; i -= APU_TICKS) {
          this.WriteAudioFrame_(1);
        }
        this.syncTicks_ += cyclesThisIteration;
      }*/
    }


    private void Update_(int cyclesThisIteration) {
      while (cyclesThisIteration > 0) {
        var next_seq_ticks =
            Apu.NextModulo_(this.syncTicks_, FRAME_SEQUENCER_TICKS);
        if (next_seq_ticks == FRAME_SEQUENCER_TICKS) {
          this.frame_ = (byte) ((this.frame_ + 1) % FRAME_SEQUENCER_COUNT);

          var updateSweep = this.frame_ == 2 || this.frame_ == 6;
          var updateLengths =
              updateSweep || this.frame_ == 0 || this.frame_ == 4;
          var updateEnvelopes = updateLengths || this.frame_ == 7;

          if (updateSweep) {
            this.UpdateSweep_();
          }

          if (updateLengths) {
            this.UpdateLengths_();
          }

          if (updateEnvelopes) {
            this.UpdateEnvelopes_();
          }
        }
        var ticks = Math.Min(next_seq_ticks, cyclesThisIteration);
        this.UpdateChannels_(ticks / APU_TICKS);
        cyclesThisIteration -= ticks;
      }
    }

    private uint GetGbFramesUntilNextResampledFrame_() {
      uint result = 0;

      uint counter = this.freqCounter_;
      while (!VALUE_WRAPPED(ref counter, APU_TICKS_PER_SECOND)) {
        counter += Apu.FREQUENCY;
        result++;
      }
      return result;
    }

    private void UpdateChannels_(int total_frames) {
      while (total_frames > 0) {
        int frames = (int) this.GetGbFramesUntilNextResampledFrame_();
        frames = Math.Min(frames, total_frames);

        this.UpdateSquareWave_(this.Channel1, frames);
        this.UpdateSquareWave_(this.Channel2, frames);
        this.UpdateWave_((uint) this.syncTicks_, frames);
        this.UpdateNoise_(frames);

        this.WriteAudioFrame_(frames);

        this.syncTicks_ += frames * APU_TICKS;
        total_frames -= frames;
      }
    }

    private void UpdateSweep_() {
      var channel1 = this.Channel1;

      var sweep = this.sweep;
      if (!(channel1.status && sweep.enabled)) {
        return;
      }

      byte period = sweep.period;
      if (--sweep.timer == 0) {
        if (period > 0) {
          sweep.timer = period;
          ushort new_frequency = this.calculate_sweep_frequency();
          if (new_frequency > SOUND_MAX_FREQUENCY) {
            channel1.status = false;
          } else {
            if (sweep.shift > 0) {
              sweep.frequency = channel1.frequency = new_frequency;
              this.write_square_wave_period(channel1, channel1.square_wave);
            }

            /* Perform another overflow check. */
            if (this.calculate_sweep_frequency() > SOUND_MAX_FREQUENCY) {
              channel1.status = false;
            }
          }
        } else {
          sweep.timer = SWEEP_MAX_PERIOD;
        }
      }
    }

    private ushort calculate_sweep_frequency() {
      var sweep = this.sweep;
      ushort f = sweep.frequency;
      if (sweep.direction == SweepDirection.SWEEP_DIRECTION_ADDITION) {
        return (ushort) (f + (f >> sweep.shift));
      }

      sweep.calculated_subtract = true;
      return (ushort) (f - (f >> sweep.shift));
    }

    private void UpdateLengths_() {
      for (var i = 0; i < APU_CHANNEL_COUNT; ++i) {
        var channel = this.channel[i];
        if (channel.length_enabled && channel.length > 0) {
          if (--channel.length == 0) {
            channel.status = false;
          }
        }
      }
    }

    private void UpdateEnvelopes_() {
      for (var i = 0; i < APU_CHANNEL_COUNT; ++i) {
        var envelope = this.channel[i].envelope;
        if (envelope.period > 0) {
          if (envelope.automatic && --envelope.timer == 0) {
            envelope.timer = envelope.period;
            byte delta = (byte) (
                                  envelope.direction ==
                                  EnvelopeDirection.ENVELOPE_ATTENUATE
                                      ? -1
                                      : 1);
            byte volume = (byte) (envelope.volume + delta);
            if (volume < ENVELOPE_MAX_VOLUME) {
              envelope.volume = volume;
            } else {
              envelope.automatic = false;
            }
          }
        } else {
          envelope.timer = ENVELOPE_MAX_PERIOD;
        }
      }
    }

    /* Convert from 1-bit sample to 4-bit sample. */
    private static byte CHANNELX_SAMPLE(Channel channel, byte sample)
      => (byte) (-sample & channel.envelope.volume);

    private void UpdateSquareWave_(Channel channel, int total_frames) {
      if (channel.status) {
        var square = channel.square_wave;
        while (total_frames > 0) {
          int frames = (int) (square.ticks / APU_TICKS);
          byte sample = Apu.CHANNELX_SAMPLE(channel, square.sample);
          if (frames <= total_frames) {
            square.ticks = square.period;
            square.position = (byte) ((square.position + 1) % DUTY_CYCLE_COUNT);
            square.sample = Apu.GetDuty(square.duty, square.position);
          } else {
            frames = total_frames;
            square.ticks -= (uint) frames * APU_TICKS;
          }
          channel.accumulator += sample * (uint) frames;
          total_frames -= frames;
        }
      }
    }

    private void UpdateWave_(uint apu_ticks, int total_frames) {
      var channel3 = this.Channel3;
      if (channel3.status) {
        var wave = this.wave;

        while (total_frames > 0) {
          int frames = (int) (wave.ticks / APU_TICKS);
          /* Modulate 4-bit sample by wave volume. */
          byte sample = (byte) (wave.sample_data >> wave.volume_shift);
          if (frames <= total_frames) {
            wave.position = (byte) ((wave.position + 1) % WAVE_SAMPLE_COUNT);
            wave.sample_time = apu_ticks + wave.ticks;
            byte b = wave.ram[wave.position >> 1];
            if ((wave.position & 1) == 0) {
              wave.sample_data = (byte) (b >> 4); /* High nybble. */
            } else {
              wave.sample_data = (byte) (b & 0x0f); /* Low nybble. */
            }
            wave.ticks = wave.period;
          } else {
            frames = total_frames;
            wave.ticks -= (uint) frames * APU_TICKS;
          }
          apu_ticks += (uint) frames * APU_TICKS;
          channel3.accumulator += sample * (uint) frames;
          total_frames -= frames;
        }
      }
    }

    public void UpdateNoise_(int total_frames) {
      var channel4 = this.Channel4;
      if (channel4.status) {
        var noise = this.noise;

        while (total_frames > 0) {
          int frames = (int) (noise.ticks / APU_TICKS);
          byte sample = Apu.CHANNELX_SAMPLE(channel4, noise.sample);
          if (noise.clock_shift <= NOISE_MAX_CLOCK_SHIFT) {
            if (frames <= total_frames) {
              ushort bit = (ushort) ((noise.lfsr ^ (noise.lfsr >> 1)) & 1);
              if (noise.lfsr_width == LfsrWidth.LFSR_WIDTH_7) {
                noise.lfsr =
                    (ushort) (((noise.lfsr >> 1) & ~0x40) | (bit << 6));
              } else {
                noise.lfsr =
                    (ushort) (((noise.lfsr >> 1) & ~0x4000) | (bit << 14));
              }
              noise.sample = (byte) (~noise.lfsr & 1);
              noise.ticks = noise.period;
            } else {
              frames = total_frames;
              noise.ticks -= (uint) frames * APU_TICKS;
            }
          } else {
            frames = total_frames;
          }
          channel4.accumulator += sample * (uint) frames;
          total_frames = (int) (total_frames - frames);
        }
      }
    }

    private int bufferPos_;
    private readonly byte[] buffer_ = new byte[BUFFER_SIZE];
    public Subject<byte[]> BufferSubject { get; } = new Subject<byte[]>();

    private void WriteAudioFrame_(int gb_frames) {
      this.divisor_ += (uint) gb_frames;
      this.freqCounter_ += Apu.FREQUENCY * (uint) gb_frames;
      if (VALUE_WRAPPED(ref this.freqCounter_, APU_TICKS_PER_SECOND)) {
        for (var i = 0; i < SOUND_OUTPUT_COUNT; ++i) {
          uint accumulator = 0;
          for (var j = 0; j < APU_CHANNEL_COUNT; ++j) {
            //if (!e->config.disable_sound[j]) {
            accumulator = (uint) (accumulator +
                                  this.channel[j].accumulator *
                                  (this.GetSoOutput(j, i) ? 1 : 0));
            //}
          }
          accumulator *= (this.so_volume[i] + 1) * 16; // 4bit -> 8bit samples.
          accumulator /= ((SOUND_OUTPUT_MAX_VOLUME + 1) * APU_CHANNEL_COUNT);
          this.buffer_[this.bufferPos_++] =
              (byte) (accumulator / this.divisor_);

          if (this.bufferPos_ >= Apu.BUFFER_SIZE) {
            this.BufferSubject.OnNext(this.buffer_);
            this.bufferPos_ = 0;
          }
        }
        for (var j = 0; j < APU_CHANNEL_COUNT; ++j) {
          this.channel[j].accumulator = 0;
        }
        this.divisor_ = 0;
      }
      //assert(buffer->position <= buffer->end);
    }

    private static int NextModulo_(int value, int mod) => mod - value % mod;

    private static bool VALUE_WRAPPED(ref uint x, uint max) {
      if (x >= max) {
        x -= max;
        return true;
      }
      return false;
    }

    public class Sweep {
      public byte period;
      public SweepDirection direction;
      public byte shift;
      public ushort frequency;
      public byte timer; /* 0..period */
      public bool enabled;
      public bool calculated_subtract;
    }

    public class Envelope {
      public byte initial_volume;
      public EnvelopeDirection direction;
      public byte period;
      public byte volume;    /* 0..15 */
      public uint timer;     /* 0..period */
      public bool automatic; /* TRUE when MAX/MIN has not yet been reached. */
    }

/* Channel 1 and 2 */
    public class SquareWave {
      public WaveDuty duty;
      public byte sample;   /* Last sample generated, 0..1 */
      public uint period;   /* Calculated from the frequency. */
      public byte position; /* Position in the duty tick, 0..7 */
      public uint ticks;    /* 0..period */
    }

/* Channel 3 */
    public class Wave {
      public WaveVolume volume;
      public byte volume_shift;
      public byte[] ram = new byte[WAVE_RAM_SIZE];
      public ulong sample_time; /* Time (in ticks) the sample was read. */
      public byte sample_data;  /* Last sample generated, 0..1 */
      public uint period;       /* Calculated from the frequency. */
      public byte position;     /* 0..31 */
      public uint ticks;        /* 0..period */

      public bool
          playing; /* TRUE if the channel has been triggered but the DAC not
                             disabled. */
    }

    public enum LfsrWidth {
      LFSR_WIDTH_15 = 0, /* 15-bit LFSR */
      LFSR_WIDTH_7 = 1,  /* 7-bit LFSR */
    }

// Channel 4
    private class Noise {
      public byte clock_shift;
      public LfsrWidth lfsr_width;
      public byte divisor; /* 0..NOISE_DIVISOR_COUNT */
      public byte sample;  /* Last sample generated, 0..1 */
      public ushort lfsr;  /* Linear feedback shift register, 15- or 7-bit. */
      public uint period;  /* Calculated from the clock_shift and divisor. */
      public uint ticks;   /* 0..period */
    }

    private class Channel {
      public SquareWave square_wave; /* Channel 1, 2 */
      public Envelope envelope;      /* Channel 1, 2, 4 */
      public ushort frequency;       /* Channel 1, 2, 3 */
      public ushort length;          /* All channels */
      public bool length_enabled;    /* All channels */
      public bool dac_enabled;
      public bool status;      /* Status bit for NR52 */
      public uint accumulator; /* Accumulates samples for resampling. */
    }

    private void write_wave_period(Channel channel) {
      this.wave.period =
          (uint) (((SOUND_MAX_FREQUENCY + 1) - channel.frequency) * 2);
    }

    private void write_square_wave_period(
        Channel channel,
        SquareWave square) {
      square.period =
          (uint) (((SOUND_MAX_FREQUENCY + 1) - channel.frequency) * 4);
    }

    private void write_noise_period() {
      var noise = this.noise;
      byte divisor = s_divisors[noise.divisor];
      //assert(NOISE.divisor < NOISE_DIVISOR_COUNT);
      noise.period = (uint) (divisor << noise.clock_shift);
    }


    public const ushort NR10_ADDRESS = 0x0; // Channel 1 sweep

    // Channel 1 sound length/wave pattern
    public const ushort NR11_ADDRESS = 0x1;

    public const ushort NR12_ADDRESS = 0x2; // Channel 1 volume envelope

    public const ushort
        NR13_ADDRESS = 0x3; // Channel 1 frequency lo                 

    public const ushort
        NR14_ADDRESS = 0x4; // Channel 1 frequency hi                 

    public const ushort
        NR21_ADDRESS = 0x6; // Channel 2 sound length/wave pattern    

    public const ushort
        NR22_ADDRESS = 0x7; // Channel 2 volume envelope              

    public const ushort
        NR23_ADDRESS = 0x8; // Channel 2 frequency lo                 

    public const ushort
        NR24_ADDRESS = 0x9; // Channel 2 frequency hi                  

    public const ushort
        NR30_ADDRESS = 0xa; // Channel 3 DAC enabled                   

    public const ushort
        NR31_ADDRESS = 0xb; // Channel 3 sound length                  

    public const ushort
        NR32_ADDRESS = 0xc; // Channel 3 select output level           

    public const ushort
        NR33_ADDRESS = 0xd; // Channel 3 frequency lo                  

    public const ushort
        NR34_ADDRESS = 0xe; // Channel 3 frequency hi                  

    public const ushort
        NR41_ADDRESS = 0x10; // Channel 4 sound length                  

    public const ushort
        NR42_ADDRESS = 0x11; // Channel 4 volume envelope               

    public const ushort
        NR43_ADDRESS = 0x12; // Channel 4 polynomial counter            

    public const ushort
        NR44_ADDRESS = 0x13; // Channel 4 counter/consecutive; trigger

    public const ushort
        NR50_ADDRESS = 0x14; // Sound volume                           

    public const ushort NR51_ADDRESS = 0x15; // Sound output select

    // Sound enabled
    public const ushort NR52_ADDRESS = 0x16;


    /*public void write_apu(ushort address, u8 value) {
      if (!this.enabled) {
        if ( //!IS_CGB &&
            (address == NR11_ADDRESS ||
             address == NR21_ADDRESS ||
             address == NR31_ADDRESS ||
             address == NR41_ADDRESS)) {
          // DMG allows writes to the length counters when power is disabled.
        } else if (address == NR52_ADDRESS) {
          // Always can write to NR52; it's necessary to re-enable power to APU.
        } else {
          // Ignore all other writes.
          return;
        }
      }

      var sweep = this.sweep;
      var wave = this.wave;
      var noise = this.noise;

//if (APU.initialized) {
      // apu_synchronize(e);
//}
      switch (address) {
        case NR10_ADDRESS: {
          SweepDirection old_direction = sweep.direction;
          sweep.period = UNPACK(value, NR10_SWEEP_PERIOD);
          sweep.direction = UNPACK(value, NR10_SWEEP_DIRECTION);
          sweep.shift = UNPACK(value, NR10_SWEEP_SHIFT);
          if (old_direction == SWEEP_DIRECTION_SUBTRACTION &&
              sweep.direction == SWEEP_DIRECTION_ADDITION &&
              sweep.calculated_subtract) {
            CHANNEL1.status = false;
          }
          break;
        }
        case NR11_ADDRESS:
          write_nrx1_reg(&CHANNEL1, address, value);
          break;
        case NR12_ADDRESS:
          write_nrx2_reg(&CHANNEL1, address, value);
          break;
        case NR13_ADDRESS:
          write_nrx3_reg(&CHANNEL1, value);
          write_square_wave_period(&CHANNEL1, &CHANNEL1.square_wave);
          break;
        case NR14_ADDRESS: {
          Bool trigger =
              write_nrx4_reg(&CHANNEL1, addr, value, NRX1_MAX_LENGTH);
          this.write_square_wave_period(&CHANNEL1, &CHANNEL1.square_wave);
          if (trigger) {
            trigger_nrx4_envelope(e, &CHANNEL1.envelope, address);
            trigger_nr14_reg(e, &CHANNEL1);
            CHANNEL1.square_wave.ticks = CHANNEL1.square_wave.period;
          }
          break;
        }
        case NR21_ADDRESS:
          write_nrx1_reg(e, &CHANNEL2, address, value);
          break;
        case NR22_ADDRESS:
          write_nrx2_reg(e, &CHANNEL2, address, value);
          break;
        case NR23_ADDRESS:
          write_nrx3_reg(e, &CHANNEL2, value);
          write_square_wave_period(e, &CHANNEL2, &CHANNEL2.square_wave);
          break;
        case NR24_ADDRESS: {
          Bool trigger =
              write_nrx4_reg(e, &CHANNEL2, addr, value, NRX1_MAX_LENGTH);
          write_square_wave_period(e, &CHANNEL2, &CHANNEL2.square_wave);
          if (trigger) {
            trigger_nrx4_envelope(e, &CHANNEL2.envelope, address);
            CHANNEL2.square_wave.ticks = CHANNEL2.square_wave.period;
          }
          break;
        }
        case NR30_ADDRESS:
          CHANNEL3.dac_enabled = UNPACK(value, NR30_DAC_ENABLED);
          if (!CHANNEL3.dac_enabled) {
            CHANNEL3.status = false;
            wave.playing = false;
          }
          break;
        case NR31_ADDRESS:
          CHANNEL3.length = NR31_MAX_LENGTH - value;
          break;
        case NR32_ADDRESS:
          wave.volume = UNPACK(value, NR32_SELECT_WAVE_VOLUME);
          //assert(wave.volume < WAVE_VOLUME_COUNT);
          wave.volume_shift = s_wave_volume_shift[wave.volume];
          break;
        case NR33_ADDRESS:
          write_nrx3_reg(e, &CHANNEL3, value);
          write_wave_period(e, &CHANNEL3);
          break;
        case NR34_ADDRESS: {
          Bool trigger =
              write_nrx4_reg(e, &CHANNEL3, addr, value, NR31_MAX_LENGTH);
          write_wave_period(e, &CHANNEL3);
          if (trigger) {
            if (!IS_CGB && WAVE.playing) {
              // Triggering the wave channel while it is already playing will
              // corrupt the wave RAM on DMG.
              if (WAVE.ticks == WAVE_TRIGGER_CORRUPTION_OFFSET_TICKS) {
                assert(WAVE.position < 32);
                u8 position = (wave.position + 1) & 31;
                u8 b = wave.ram[position >> 1];
                switch (position >> 3) {
                  case 0:
                    wave.ram[0] = b;
                    break;
                  case 1:
                  case 2:
                  case 3:
                    memcpy(&wave.ram[0], &wave.ram[(position >> 1) & 12], 4);
                    break;
                }
              }
            }

            wave.position = 0;
            wave.ticks = wave.period + WAVE_TRIGGER_DELAY_TICKS;
            wave.playing = true;
          }
          break;
        }
        case NR41_ADDRESS:
          write_nrx1_reg(e, &CHANNEL4, addr, value);
          break;
        case NR42_ADDRESS:
          write_nrx2_reg(e, &CHANNEL4, addr, value);
          break;
        case NR43_ADDRESS: {
          noise.clock_shift = UNPACK(value, NR43_CLOCK_SHIFT);
          noise.lfsr_width = UNPACK(value, NR43_LFSR_WIDTH);
          noise.divisor = UNPACK(value, NR43_DIVISOR);
          write_noise_period(e);
          break;
        }
        case NR44_ADDRESS: {
          Bool trigger =
              write_nrx4_reg(e, &CHANNEL4, addr, value, NRX1_MAX_LENGTH);
          if (trigger) {
            this.write_noise_period();
            trigger_nrx4_envelope(e, &CHANNEL4.envelope, addr);
            noise.lfsr = 0x7fff;
            noise.sample = 1;
            noise.ticks = noise.period;
          }
          break;
        }
        case NR50_ADDRESS:
          APU.so_output[VIN][1] = UNPACK(value, NR50_VIN_SO2);
          APU.so_volume[1] = UNPACK(value, NR50_SO2_VOLUME);
          APU.so_output[VIN][0] = UNPACK(value, NR50_VIN_SO1);
          APU.so_volume[0] = UNPACK(value, NR50_SO1_VOLUME);
          break;
        case NR51_ADDRESS:
          APU.so_output[SOUND4][1] = UNPACK(value, NR51_SOUND4_SO2);
          APU.so_output[SOUND3][1] = UNPACK(value, NR51_SOUND3_SO2);
          APU.so_output[SOUND2][1] = UNPACK(value, NR51_SOUND2_SO2);
          APU.so_output[SOUND1][1] = UNPACK(value, NR51_SOUND1_SO2);
          APU.so_output[SOUND4][0] = UNPACK(value, NR51_SOUND4_SO1);
          APU.so_output[SOUND3][0] = UNPACK(value, NR51_SOUND3_SO1);
          APU.so_output[SOUND2][0] = UNPACK(value, NR51_SOUND2_SO1);
          APU.so_output[SOUND1][0] = UNPACK(value, NR51_SOUND1_SO1);
          break;
        case NR52_ADDRESS: {
          bool was_enabled = this.enabled;
          bool is_enabled = UNPACK(value, NR52_ALL_SOUND_ENABLED);
          if (was_enabled && !is_enabled) {
            int i;
            for (i = 0; i < APU_REG_COUNT; ++i) {
              if (i != NR52_ADDRESS) {
                write_apu(e, i, 0);
              }
            }
          } else if (!was_enabled && is_enabled) {
            this.frame_ = 7;
          }
          this.enabled = is_enabled;
          break;
        }
      }
    }

    private void write_nrx1_reg(Channel channel, Address addr, byte value) {
      if (this.enabled) {
        channel.square_wave.duty = UNPACK(value, NRX1_WAVE_DUTY);
      }
      channel.length = NRX1_MAX_LENGTH - UNPACK(value, NRX1_LENGTH);
    }

    private void write_nrx2_reg(Channel channel, ushort address, byte value) {
      channel.envelope.initial_volume = UNPACK(value, NRX2_INITIAL_VOLUME);
      channel.dac_enabled = UNPACK(value, NRX2_DAC_ENABLED) != 0;
      if (!channel.dac_enabled) {
        channel.status = false;
      }
      if (channel.status) {
        if (channel.envelope.period == 0 && channel->envelope.automatic) {
          byte new_volume =
              (channel->envelope.volume + 1) & ENVELOPE_MAX_VOLUME;
          channel.envelope.volume = new_volume;
        }
      }
      channel.envelope.direction = UNPACK(value, NRX2_ENVELOPE_DIRECTION);
      channel.envelope.period = UNPACK(value, NRX2_ENVELOPE_PERIOD);
    }

    private void write_nrx3_reg(Channel channel, byte value) {
      channel.frequency = (ushort) ((channel.frequency & ~0xff) | value);
    }*/
  }
}