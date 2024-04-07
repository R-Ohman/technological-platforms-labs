package org.example;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.util.Optional;

import static org.hamcrest.MatcherAssert.assertThat;
import static org.hamcrest.Matchers.is;
import static org.mockito.Mockito.*;

public class MageControllerTests {
    private MageController controller;
    private MageRepository repository;

    @BeforeEach
    public void setUp() {
        repository = mock(MageRepository.class);
        controller = new MageController(repository);
    }

    @Test
    public void findExists() {
        when(repository.find("Gandalf")).thenReturn(Optional.of(new Mage("Gandalf", 20)));

        var result = controller.find("Gandalf");
        assertThat(result, is("Mage{name='Gandalf', level=20}"));
    }

    @Test
    public void findNotExists() {
        when(repository.find("Voldemort")).thenReturn(Optional.empty());

        var result = controller.find("Voldemort");
        assertThat(result, is("not found"));
    }

    @Test
    public void deleteExists() {
        var result = controller.delete("Gandalf");
        verify(repository).delete("Gandalf");
        assertThat(result, is("done"));
    }

    @Test
    public void deleteNotExists() {
        doThrow(new IllegalArgumentException("Mage Voldemort not found")).when(repository).delete("Voldemort");

        var result = controller.delete("Voldemort");
        assertThat(result, is("not found"));
    }

    @Test
    public void saveNotExists() {
        var result = controller.save("Voldemort", "40");
        verify(repository).save(new Mage("Voldemort", 40));
        assertThat(result, is("done"));
    }

    @Test
    public void saveExists() {
        doThrow(new IllegalArgumentException("Mage Gandalf already exists")).when(repository).save(new Mage("Gandalf", 40));

        var result = controller.save("Gandalf", "40");
        assertThat(result, is("bad request"));
    }
}
