package org.example;

import java.util.Comparator;

public class MageComparator implements Comparator<Mage> {

    @Override
    public int compare(Mage o1, Mage o2) {
        int levelComparison = Integer.compare(o1.getLevel(), o2.getLevel());
        if (levelComparison != 0) {
            return levelComparison;
        }
        int nameComparison = o1.getName().compareTo(o2.getName());
        if (nameComparison != 0) {
            int nameLengthComparison = Integer.compare(o1.getName().length(), o2.getName().length());
            if (nameLengthComparison != 0) {
                return nameLengthComparison;
            }
            return nameComparison;
        }
        return Double.compare(o1.getPower(), o2.getPower());
    }
}
