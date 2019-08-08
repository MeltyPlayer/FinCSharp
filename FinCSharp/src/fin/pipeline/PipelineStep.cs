using System;
using System.Threading.Tasks;

namespace fin.pipeline {
  public static class Pipeline {
    public static IPipelineStep<INPUT_TYPE> Step<INPUT_TYPE>(
      Action<INPUT_TYPE> handler) {
      return new SyncPipelineCap<INPUT_TYPE>(handler);
    }

    public static IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> Step
      <INPUT_TYPE, OUTPUT_TYPE>(Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler) {
      return new AsyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE>(handler);
    }

    public static IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> Step
      <INPUT_TYPE, OUTPUT_TYPE>(Func<INPUT_TYPE, OUTPUT_TYPE> handler) {
      return new SyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE>(handler);
    }
  }

  class SyncPipelineCap<INPUT_TYPE> : IPipelineStep<INPUT_TYPE> {
    private readonly Action<INPUT_TYPE> handler_;

    public SyncPipelineCap(Action<INPUT_TYPE> handler) {
      handler_ = handler;
    }

    public Task Call(INPUT_TYPE inputValue) {
      handler_(inputValue);
      return Task.CompletedTask;
    }
  }

  class AsyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE> :
    IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> {
    private readonly Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler_;

    public AsyncPipelineStep(Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler) {
      handler_ = handler;
    }

    protected override async Task<OUTPUT_TYPE> Convert(INPUT_TYPE inputValue) {
      return await handler_(inputValue);
    }
  }

  class SyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE> :
    IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> {
    private readonly Func<INPUT_TYPE, OUTPUT_TYPE> handler_;

    public SyncPipelineStep(Func<INPUT_TYPE, OUTPUT_TYPE> handler) {
      handler_ = handler;
    }

    protected override Task<OUTPUT_TYPE> Convert(INPUT_TYPE inputValue) {
      return Task.FromResult(handler_(inputValue));
    }
  }
}