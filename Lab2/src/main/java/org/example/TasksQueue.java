package org.example;

import java.util.LinkedList;
import java.util.Optional;
import java.util.Queue;

public class TasksQueue {
    private final Queue<Task> tasks = new LinkedList<>();
    private int taskId = 0;

    public synchronized void addTask(int fibNumber) {
        tasks.add(new Task(taskId++, fibNumber));
        notify();
    }

    public synchronized Optional<Task> getTask() {
        // Wait for a task and return it
        while (tasks.isEmpty()) {
            try {
                wait();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                return Optional.empty();
            }
        }
        return Optional.of(tasks.poll());
    }
}
