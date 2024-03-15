package org.example;

import java.io.IOException;
import java.net.ServerSocket;

public class Server {
    public static void main(String[] args) {
        try (var server = new ServerSocket(4321)) {

            System.out.println("Server started on localhost:4321. Waiting for connections...");
            while (true) {
                var client = server.accept();
                var connectionWorker = new Thread(new ConnectionWorker(client));
                connectionWorker.start();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
