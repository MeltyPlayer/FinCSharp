using System;
using System.Threading.Tasks;

namespace fin.pipeline {
  public static class Pipeline {
    public static IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> Step<INPUT_TYPE, OUTPUT_TYPE>(Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler) {
      return new AsyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE>(handler);
    }
  }

  class AsyncPipelineStep<INPUT_TYPE, OUTPUT_TYPE> : IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> {
    private readonly Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler_;

    public AsyncPipelineStep(Func<INPUT_TYPE, Task<OUTPUT_TYPE>> handler) {
      handler_ = handler;
    }

    protected override async Task<OUTPUT_TYPE> Convert(INPUT_TYPE inputValue) {
      return await handler_(inputValue);
    }
  }
}
