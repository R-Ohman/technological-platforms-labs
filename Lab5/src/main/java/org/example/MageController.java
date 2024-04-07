package org.example;

public class MageController {
    private final MageRepository repository;

    public MageController(MageRepository repository) {
        this.repository = repository;
    }

    public String find(String name) {
        return repository.find(name)
                .map(Mage::toString)
                .orElse("not found");
    }

    public String delete(String name) {
        try {
            repository.delete(name);
            return "done";
        }
        catch (IllegalArgumentException e) {
            return "not found";
        }
    }

    public String save(String name, String level) {
        try {
            Mage mage = new Mage(name, Integer.parseInt(level));
            repository.save(mage);
            return "done";
        }
        catch (IllegalArgumentException e) {
            return "bad request";
        }
    }
}