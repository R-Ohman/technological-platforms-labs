package org.example;

import java.util.List;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static com.github.npathai.hamcrestopt.OptionalMatchers.isEmpty;
import static com.github.npathai.hamcrestopt.OptionalMatchers.isPresent;
import static org.hamcrest.MatcherAssert.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

public class MageRepositoryTests {
    private MageRepository repository;

    @BeforeEach
    public void setUp() {
        repository = new MageRepository(List.of(
                new Mage("Merlin", 10),
                new Mage("Gandalf", 20),
                new Mage("Dumbledore", 30)
        ));
    }

    @Test
    public void findExists() {
        var mage = repository.find("Gandalf");
        assertThat(mage, isPresent());
    }

    @Test
    public void findNotExists() {
        var mage = repository.find("Voldemort");
        assertThat(mage, isEmpty());
    }

    @Test
    public void deleteExists() {
        repository.delete("Gandalf");
        var mage = repository.find("Gandalf");
        assertThat(mage, isEmpty());
    }

    @Test
    public void deleteNotExists() {
        assertThrows(IllegalArgumentException.class, () -> repository.delete("Voldemort"));
    }

    @Test
    public void saveNotExists() {
        repository.save(new Mage("Voldemort", 40));
        var mage = repository.find("Voldemort");
        assertThat(mage, isPresent());
    }

    @Test
    public void saveExists() {
        assertThrows(IllegalArgumentException.class, () -> repository.save(new Mage("Gandalf", 40)));
    }
}
