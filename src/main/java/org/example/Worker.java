package org.example;

import java.util.Optional;

public class Worker implements Runnable {
    private final TasksQueue tasksQueue;
    private final ResultsQueue resultsQueue;

    public Worker(TasksQueue tasksQueue, ResultsQueue resultsQueue) {
        this.tasksQueue = tasksQueue;
        this.resultsQueue = resultsQueue;
    }

    @Override
    public void run() {
        while (true) {
            long i = 1;
            long result = 1;
            Optional<Task> task = null;

            try {
                task = tasksQueue.getTask();
                if (task.isEmpty()) {
                    return;
                }

                long fibPrev = 0;
                long fibNext;

                for (i = 2; i <= task.get().fibNumber(); i++) {
                    fibNext = fibPrev + result;
                    fibPrev = result;
                    result = fibNext;
                    Thread.sleep(200);
                }

                resultsQueue.addResult(TaskResult.Success(task.get().taskId(), result));
            } catch (InterruptedException e) {
                int percentCompleted = (int) ( (double) i / task.get().fibNumber()) * 100;
                resultsQueue.addResult(TaskResult.Error(task.get().taskId(), percentCompleted, result));
                Thread.currentThread().interrupt();
                break;
            }
        }
    }
}
