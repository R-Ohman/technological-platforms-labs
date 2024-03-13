package org.example;

import java.util.Optional;
import java.util.Queue;
import java.util.LinkedList;

public class ResultsQueue {
    private final Queue<TaskResult> results = new LinkedList<>();

    public synchronized void addResult(TaskResult result) {
        results.add(result);
        notify();
    }

    public synchronized Optional<TaskResult> getResult() {
        // Wait for a result and return it
        while (results.isEmpty()) {
            try {
                wait();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                return Optional.empty();
            }
        }
        return Optional.of(results.poll());
    }

    public synchronized boolean isEmpty() {
        return results.isEmpty();
    }
}
