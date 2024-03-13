package org.example;

public class TaskResult {
    private final int taskId;
    private final long percentCompleted;
    private final long resultValue;

    private TaskResult(int taskId, long percentCompleted, long resultValue) {
        this.taskId = taskId;
        this.percentCompleted = percentCompleted;
        this.resultValue = resultValue;
    }

    public static TaskResult Success(int taskId, long result) {
        return new TaskResult(taskId, 100, result);
    }

    public static TaskResult Error(int taskId, int percentCompleted, long resultValue) {
        return new TaskResult(taskId, percentCompleted, resultValue);
    }

    @Override
    public String toString() {
        return "[ID:%d] Returned %d with %d percent completed".formatted(taskId, resultValue, percentCompleted);
    }
}
