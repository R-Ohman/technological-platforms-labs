package org.example;

import javax.imageio.ImageIO;

import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Main {
    public static void main(String[] args) {
        if (args.length < 2) {
            System.err.println("Usage: java -jar <jar-file> <input-dir> <output-dir>");
            System.exit(1);
        }

        Path inputDir = Path.of(args[0]);
        Path outputDir = Path.of(args[1]);

        int numThreads = Runtime.getRuntime().availableProcessors();
        if (args.length == 3)
            numThreads = Integer.parseInt(args[2]);

        long startTime = System.currentTimeMillis();

        try (var stream = Files.list(inputDir)) {
            if (!Files.exists(outputDir))
                Files.createDirectory(outputDir);

            ExecutorService threadPool = Executors.newFixedThreadPool(numThreads);
            CompletableFuture<?>[] futures = stream
                    .map(path -> CompletableFuture.runAsync(() -> ProcessImage(path, outputDir), threadPool))
                    .toArray(CompletableFuture[]::new);
            CompletableFuture.allOf(futures).join();

            threadPool.shutdown();
        } catch (IOException e) {
            System.err.println("An error occurred while processing the images: " + e.getMessage());
        }

        long endTime = System.currentTimeMillis();
        double duration = (endTime - startTime) / 1000.0;
        System.out.printf("Execution time: %.2f seconds%n", duration);
    }

    private static Image LoadImage(Path path) {
        try {
            return new Image(path.getFileName().toString(), ImageIO.read(path.toFile()));
        } catch (IOException e) {
            System.err.println("An error occurred while loading the image: " + e.getMessage());
            return null;
        }
    }

    private static void ProcessImage(Path path, Path outputDir) {
        Image image = LoadImage(path);
        if (image != null) {
            BufferedImage processedImage = ConvertImage(image.data());
            SaveImage(new Image(image.name(), processedImage), outputDir);
        }
    }

    private static BufferedImage ConvertImage(BufferedImage image) {
        var negativeGreyImage = new BufferedImage(image.getWidth(), image.getHeight(), BufferedImage.TYPE_INT_ARGB);
        for (int x = 0; x < image.getWidth(); x++) {
            for (int y = 0; y < image.getHeight(); y++) {
                var color = new Color(image.getRGB(x, y));
                var grey = (color.getRed() + color.getGreen() + color.getBlue()) / 3;
                var newColor = new Color(255 - grey, 255 - grey, 255 - grey);
                negativeGreyImage.setRGB(x, y, newColor.getRGB());
            }
        }
        return negativeGreyImage;
    }

    private static void SaveImage(Image image, Path outputDir) {
        try {
            ImageIO.write(image.data(), "png", outputDir.resolve(image.name()).toFile());
            System.out.println("Image " + image.name() + " has been processed");
        } catch (IOException e) {
            System.err.println("An error occurred while saving the image: " + e.getMessage());
        }
    }
}