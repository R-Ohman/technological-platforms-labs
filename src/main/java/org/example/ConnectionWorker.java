package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.net.SocketException;

public class ConnectionWorker implements Runnable {
    private final Socket client;
    private final ObjectOutputStream outputStream;
    private final ObjectInputStream inputStream;

    public ConnectionWorker(Socket client) throws IOException {
        this.client = client;
        outputStream = new ObjectOutputStream(client.getOutputStream());
        inputStream = new ObjectInputStream(client.getInputStream());
    }

    @Override
    public void run() {
        try {
            System.out.printf("Connected to client at %s:%d%n", client.getInetAddress(), client.getPort());
            outputStream.writeObject("ready");
            var n = (int) inputStream.readObject();
            outputStream.writeObject("ready for messages");
            for (int i = 0; i < n; i++) {
                var message = (Message) inputStream.readObject();
                System.out.println(client.getInetAddress() + ":" + client.getPort() + " says: " + message);
            }
            outputStream.writeObject("finished");
            System.out.printf("Closing connection to client at %s:%d%n", client.getInetAddress(), client.getPort());
            close();
        }
        catch (SocketException e ) {
            if (e.getCause() == null) {
                System.out.printf("Connection to client at %s:%d closed%n", client.getInetAddress(), client.getPort());
            } else {
                e.printStackTrace();
            }
        }
        catch (ClassNotFoundException e) {
            throw new RuntimeException(e);
        }
        catch (IOException e) {
            e.printStackTrace();
        }
    }

    private void close() {
        try {
            inputStream.close();
            outputStream.close();
            client.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
