package org.example;

import java.util.HashSet;
import java.util.Objects;
import java.util.Set;
import java.util.TreeSet;

public class Mage implements Comparable<Mage> {
    private String name;
    private int level;
    private double power;
    private Set<Mage> apprentices;

    public Mage(String name, int level, double power, Set<Mage> apprentices, String sortType) {
        this.name = name;
        this.level = level;
        this.power = power;

        if (sortType.equals("no")) {
            this.apprentices = new HashSet<>(apprentices);
        } else{
            if (sortType.equals("alternative")) {
                this.apprentices = new TreeSet<>(new MageComparator());
                this.apprentices.addAll(apprentices);
            } else {
                this.apprentices = new TreeSet<>();
                this.apprentices.addAll(apprentices);
            }
        }
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        Mage mage = (Mage) o;
        return level == mage.level &&
                Double.compare(mage.power, power) == 0 &&
                Objects.equals(name, mage.name);
    }

    @Override
    public int hashCode() {
        return Objects.hash(name, level, power);
    }

    @Override
    public int compareTo(Mage other) {
        int nameComparison = this.name.compareTo(other.name);
        if (nameComparison != 0) {
            int nameLengthComparison = Integer.compare(this.name.length(), other.name.length());
            if (nameLengthComparison != 0) {
                return nameLengthComparison;
            }
            return nameComparison;
        }
        int levelComparison = Integer.compare(this.level, other.level);
        if (levelComparison != 0) {
            return levelComparison;
        }
        return Double.compare(this.power, other.power);
    }

    @Override
    public String toString() {
        return "Mage{name='" + name + "', level=" + level + ", power=" + power + "}";
    }

    public int getDescendantsNumber() {
        return countDescendants(this, new HashSet<>());
    }

    private int countDescendants(Mage mage, Set<Mage> visited) {
        int descendants = 0;
        for (Mage apprentice : mage.getApprentices()) {
            if (!visited.contains(apprentice)) {
                visited.add(apprentice);
                descendants += 1 + countDescendants(apprentice, visited);
            }
        }
        return descendants;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getLevel() {
        return level;
    }

    public void setLevel(int level) {
        this.level = level;
    }

    public double getPower() {
        return power;
    }

    public void setPower(double power) {
        this.power = power;
    }

    public Set<Mage> getApprentices() {
        return apprentices;
    }

    public void setApprentices(Set<Mage> apprentices) {
        this.apprentices = apprentices;
    }
}

