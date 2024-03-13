package org.example;

import java.util.Arrays;
import java.util.Scanner;
import java.util.stream.IntStream;

public class Main {
    public static void main(String[] args) {
        var tasksQueue = new TasksQueue();
        var resultsQueue = new ResultsQueue();
        var scanner = new Scanner(System.in);

        var workersNumber = args.length > 0 ? Integer.parseInt(args[0]) : 6;
        var workers = IntStream.range(0, workersNumber + 1)
                .mapToObj(i -> new Worker(tasksQueue, resultsQueue))
                .map(Thread::new)
                .peek(Thread::start)
                .toArray(Thread[]::new);

        var printWorker = new Thread(new PrintWorker(resultsQueue));
        printWorker.start();

        // Add some random tasks
        for (var number : new java.util.Random().ints(4, 2, 250).toArray()) {
            tasksQueue.addTask(number);
            System.out.println("[+] Added " + number + " to the tasks queue");
        }

        while (true) {
            var userInput = scanner.nextLine();
            if (userInput.equals("exit")) {
                for (var worker : workers) {
                    worker.interrupt();
                }
                // Wait for all workers to finish
                while(Arrays.stream(workers).anyMatch(Thread::isAlive)) {
                    try {
                        Thread.sleep(250);
                    } catch (InterruptedException e) {
                        throw new RuntimeException(e);
                    }
                }
                printWorker.interrupt();
                break;
            }
            // Add user input to the tasks queue
            try {
                var number = Integer.parseInt(userInput);
                tasksQueue.addTask(number);
                System.out.println("[+] Added " + number + " to the queue");
            } catch (NumberFormatException e) {
                System.out.println("[-] Invalid number");
            }
        }
    }
}