package org.example;

import java.util.*;
import java.util.AbstractMap.SimpleEntry;

public class Main {
    public static void main(String[] args) {
        // sortType in ["no", "alternative", "natural"]
        String sortType = args.length > 0 ? args[0] : "no";

        List<Mage> mages = new ArrayList<>();

        mages.add(new Mage("Mage0", 9, 100, Set.of(), sortType));
        mages.add(new Mage("Mage1", 3, 90, Set.of(), sortType));
        mages.add(new Mage("Mage2", 3, 100, Set.of(), sortType));

        mages.add(new Mage("Mage3", 7, 100, Set.of(), sortType));
        mages.add(new Mage("Mage4", 2, 100, Set.of(mages.get(0)), sortType));
        mages.add(new Mage("Mage5", 5, 100, Set.of(), sortType));
        mages.add(new Mage("Mage6", 5, 90, Set.of(mages.get(2), mages.get(1)), sortType));
        mages.add(new Mage("Mage7", 2, 90, Set.of(), sortType));

        mages.add(new Mage("Mage8", 4, 120, Set.of(mages.get(4), mages.get(1), mages.get(0), mages.get(3)), sortType));
        mages.add(new Mage("Mage9", 1, 120, Set.of(mages.get(5), mages.get(0), mages.get(7), mages.get(6)), sortType));

        System.out.println("Sort type: " + sortType);

        printMages(mages);

        printMageStatistics(mages, !sortType.equals("no"));
    }


    private static void printMages(List<Mage> mages) {
        Stack<SimpleEntry<Mage, Integer>> stack = new Stack<>();
        // Mages who are not apprentices of any other mage (heads of independent forests)
        for (Mage mage : mages) {
            if (mages.stream().noneMatch(m -> m.getApprentices().contains(mage))) {
                stack.add(new SimpleEntry<>(mage, 1));
            }
        }

        int level;
        while (!stack.isEmpty()) {
            SimpleEntry<Mage, Integer> currentEntry = stack.pop();
            Mage currentMage = currentEntry.getKey();
            level = currentEntry.getValue();

            for (int i = 0; i < level; i++) {
                System.out.print("-");
            }
            int finalLevel = level;

            System.out.println(currentMage);
            Stack<SimpleEntry<Mage, Integer>> apprentices = new Stack<>();

            for (Mage apprentice: currentMage.getApprentices()) {
                apprentices.add(new SimpleEntry<>(apprentice, finalLevel + 1));
            }

            Collections.reverse(apprentices);
            stack.addAll(apprentices);
        }
    }

    private static Map<Mage, Integer> getMageStatistics(List<Mage> mages, boolean sort) {
        Map<Mage, Integer> statistics = (sort) ? new TreeMap<>() : new HashMap<>();
        for (Mage mage : mages) {
            statistics.put(mage, mage.getDescendantsNumber());
        }
        return statistics;
    }

    private static void printMageStatistics(List<Mage> mages, boolean sort) {
        Map<Mage, Integer> statistics = getMageStatistics(mages, sort);
        System.out.println("Mage statistics:");
        statistics.forEach((key, value) -> System.out.println(key + " has " + value + " apprentices"));
    }
}
