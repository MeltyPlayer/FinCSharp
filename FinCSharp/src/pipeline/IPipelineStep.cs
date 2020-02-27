/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fin.pipeline {
  public interface IPipelineStep<in INPUT_TYPE> {
    Task Call(INPUT_TYPE inputValue);
  }

  public abstract class IPipelineStep<INPUT_TYPE, OUTPUT_TYPE> :
    IPipelineStep<INPUT_TYPE> {
    private readonly IList<IPipelineStep<OUTPUT_TYPE>> nextSteps_ =
      new List<IPipelineStep<OUTPUT_TYPE>>();

    protected abstract Task<OUTPUT_TYPE> Convert(INPUT_TYPE inputValue);

    public async Task Call(INPUT_TYPE inputValue) {
      OUTPUT_TYPE outputValue = await Convert(inputValue);

      Task[] nextTasks = new Task[nextSteps_.Count];
      for (int i = 0; i < nextSteps_.Count; ++i) {
        nextTasks[i] = nextSteps_[i].Call(outputValue);
      }

      await Task.WhenAll(nextTasks);
    }

    public void Then(params Action<OUTPUT_TYPE>[] handlers) {
      foreach (Action<OUTPUT_TYPE> handler in handlers) {
        Then(Pipeline.Step(handler));
      }
    }

    public void Then(IPipelineStep<OUTPUT_TYPE> step) {
      // TODO: Detect cycles.
      /*if (typeof(OUTPUT_TYPE) == typeof(NEXT_OUTPUT_TYPE) && step.nextSteps_.Contains(this)) {
        throw new exception.CycleException("Cycle detected in pipeline.");
      }

      if (nextSteps_.Contains(step)) {
        throw new exception.DuplicateInstanceException(
          "Next step is already present in pipeline.");
      }

      nextSteps_.Add(step);
    }

    public void Then<NEXT_OUTPUT_TYPE>(
      params Func<OUTPUT_TYPE, Task<NEXT_OUTPUT_TYPE>>[] handlers) {
      foreach (Func<OUTPUT_TYPE, Task<NEXT_OUTPUT_TYPE>> handler in handlers) {
        Then(Pipeline.Step(handler));
      }
    }

    public void Then<NEXT_OUTPUT_TYPE>(
      params Func<OUTPUT_TYPE, NEXT_OUTPUT_TYPE>[] handlers) {
      foreach (Func<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> handler in handlers) {
        Then(Pipeline.Step(handler));
      }
    }

    public void Then<NEXT_OUTPUT_TYPE>(
      params IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE>[] steps) {
      foreach (IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> step in steps) {
        Then(step);
      }
    }

    public IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> Then<NEXT_OUTPUT_TYPE>(
      Func<OUTPUT_TYPE, Task<NEXT_OUTPUT_TYPE>> handler) {
      return Then(Pipeline.Step(handler));
    }

    public IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> Then<NEXT_OUTPUT_TYPE>(
      Func<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> handler) {
      return Then(Pipeline.Step(handler));
    }

    public IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> Then<NEXT_OUTPUT_TYPE>(
      IPipelineStep<OUTPUT_TYPE, NEXT_OUTPUT_TYPE> step) {
      // TODO: Detect cycles.
      /*if (typeof(OUTPUT_TYPE) == typeof(NEXT_OUTPUT_TYPE) && step.nextSteps_.Contains(this)) {
        throw new exception.CycleException("Cycle detected in pipeline.");
      }

      if (nextSteps_.Contains(step)) {
        throw new exception.DuplicateInstanceException(
          "Next step is already present in pipeline.");
      }

      nextSteps_.Add(step);
      return step;
    }
  }
}*/

