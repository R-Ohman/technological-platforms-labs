package org.example;

public class PrintWorker implements Runnable {
    private final ResultsQueue resultsQueue;

    public PrintWorker(ResultsQueue resultsQueue) {
        this.resultsQueue = resultsQueue;
    }

    @Override
    public void run() {
        while (true) {
            try {
                Thread.sleep(10);
                var result = resultsQueue.getResult();
                if (result.isEmpty()) {
                    continue;
                }
                // Result is ready
                System.out.println(result.get().toString());
            } catch (InterruptedException e) {
                // Print results if any
                while (!resultsQueue.isEmpty()) {
                    var result = resultsQueue.getResult();
                    if (result.isEmpty()) {
                        break;
                    }
                    System.out.println(result.get().toString());
                }
                System.out.println("[-] PrintWorker interrupted");
                Thread.currentThread().interrupt();
                break;
            }
        }
    }
}
