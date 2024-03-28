package org.example.models;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.ManyToOne;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@NoArgsConstructor
@Getter @Setter
@Entity
public class Mage {
    @Id
    private String name;

    private int level;
    @ManyToOne
    private Tower tower;

    public Mage(String name, int level) {
        this.name = name;
        this.level = level;
    }

    @Override
    public String toString() {
        return "Mage{" +
                "name='" + name + '\'' +
                ", level=" + level +
                ", tower=" + (tower == null ? "null" : tower.getName()) +
                '}';
    }
}

