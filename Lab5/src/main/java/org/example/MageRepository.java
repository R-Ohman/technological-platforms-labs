package org.example;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Optional;

public class MageRepository {
    private final Collection<Mage> collection = new ArrayList<>();

    public MageRepository(Collection<Mage> collection) {
        this.collection.addAll(collection);
    }
    public Optional<Mage> find(String name) {
        try {
            return collection.stream()
                    .filter(mage -> mage.getName().equals(name))
                    .findFirst();
        }
        catch (IllegalArgumentException e) {
            return Optional.empty();
        }
    }
    public void delete(String name) {
        var mage = find(name);
        if (mage.isEmpty()) {
            throw new IllegalArgumentException("Mage " + name + " not found");
        }
        collection.remove(mage.get());
    }
    public void save(Mage mage) {
        if (collection.stream().anyMatch(m -> m.getName().equals(mage.getName()))) {
            throw new IllegalArgumentException("Mage " + mage.getName() + " already exists");
        }
        collection.add(mage);
    }
}