package org.example.models;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import java.util.LinkedList;
import java.util.List;

@NoArgsConstructor
@Getter @Setter
@Entity
public class Tower {
    @Id
    private String name;

    private int height;
    @OneToMany
    private List<Mage> mages;

    public Tower(String name, int height) {
        this.name = name;
        this.height = height;
        this.mages = new LinkedList<>();
    }

    @Override
    public String toString() {
        return "Tower{" +
                "name='" + name + '\'' +
                ", height=" + height +
                ", mages=" + mages +
                '}';
    }

    public void addMage(Mage mage) {
        mages.add(mage);
    }

    public void removeMage(Mage mage) {
        mages.remove(mage);
    }

}