package org.example;

import jakarta.persistence.EntityManager;
import jakarta.persistence.EntityManagerFactory;
import jakarta.persistence.EntityTransaction;
import jakarta.persistence.Persistence;
import org.example.models.*;
import java.util.Scanner;

public class Main {

    private static void transaction(EntityManager em, Runnable action) {
        var tx = em.getTransaction();
        tx.begin();
        action.run();
        tx.commit();
    }

    private static void addData(EntityManager em) {
        EntityTransaction tx = em.getTransaction();
        tx.begin();

        var mages = new Mage[] {
            new Mage("Gandalf", 100),
            new Mage("Saruman", 90),
            new Mage("Radagast", 80),
            new Mage("Alatar", 70),
            new Mage("Pallando", 60),
        };
        var towers = new Tower[] {
            new Tower("Orthanc", 100),
            new Tower("Barad-dur", 90),
            new Tower("Minas Morgul", 80)
        };

        towers[0].addMage(mages[0]);
        towers[0].addMage(mages[1]);
        towers[1].addMage(mages[2]);
        towers[1].addMage(mages[3]);

        mages[0].setTower(towers[0]);
        mages[1].setTower(towers[0]);
        mages[2].setTower(towers[1]);
        mages[3].setTower(towers[1]);

        for (var tower : towers) {
            em.persist(tower);
        }

        for (var mage : mages) {
            em.persist(mage);
        }

        tx.commit();
    }

    private static boolean processCommand(EntityManager em, Scanner scanner, int command) {
        switch (command) {
            case 0 -> {
                return false;
            }
            case 1 -> {
                var mages = em.createQuery("FROM Mage", Mage.class).getResultList();
                mages.forEach(System.out::println);
            }
            case 2 -> {
                var towers = em.createQuery("FROM Tower", Tower.class).getResultList();
                towers.forEach(System.out::println);
            }
            case 3 -> {
                System.out.println("Enter the mage's name:");
                var name = scanner.nextLine();
                System.out.println("Enter the mage's level:");
                int level = Integer.parseInt(scanner.nextLine());
                var mage = new Mage(name, level);
                transaction(em, () -> em.persist(mage));
            }
            case 4 -> {
                System.out.println("Enter the tower's name:");
                var name = scanner.nextLine();
                System.out.println("Enter the tower's height:");
                int height = Integer.parseInt(scanner.nextLine());
                var tower = new Tower(name, height);
                transaction(em, () -> em.persist(tower));
            }
            case 5 -> {
                System.out.println("Enter the mage's name:");
                var mageName = scanner.nextLine();
                System.out.println("Enter the tower's name:");
                var towerName = scanner.nextLine();
                var mage = em.find(Mage.class, mageName);
                var tower = em.find(Tower.class, towerName);

                transaction(em, () -> {
                    tower.addMage(mage);
                    mage.setTower(tower);
                    em.persist(tower);
                    em.persist(mage);
                });
            }
            case 6 -> {
                System.out.println("Enter the mage's name:");
                var name = scanner.nextLine();
                var mage = em.find(Mage.class, name);
                var tower = mage.getTower();
                transaction(em, () -> {
                    tower.removeMage(mage);
                    em.persist(tower);
                    em.remove(mage);
                });
            }
            case 7 -> {
                System.out.println("Enter the tower's name:");
                var name = scanner.nextLine();
                System.out.println(name);
                var tower = em.find(Tower.class, name);
                transaction(em, () -> {
                    for (var mage : tower.getMages()) {
                        mage.setTower(null);
                        em.persist(mage);
                    }
                    em.remove(tower);
                });
            }
            case 8 -> {
                System.out.println("Enter the mage's name:");
                var mageName = scanner.nextLine();
                var mage = em.find(Mage.class, mageName);
                var tower = em.find(Tower.class, mage.getTower());
                transaction(em, () -> {
                    tower.removeMage(mage);
                    em.persist(tower);
                });
            }
            case 9 -> {
                System.out.println("Enter the level:");
                int level = Integer.parseInt(scanner.nextLine());
                var mages = em.createQuery("FROM Mage m WHERE m.level > :level", Mage.class)
                        .setParameter("level", level)
                        .getResultList();
                mages.forEach(System.out::println);
            }
            case 10 -> {
                System.out.println("Enter the height:");
                int height = Integer.parseInt(scanner.nextLine());
                var towers = em.createQuery("FROM Tower t WHERE t.height < :height", Tower.class)
                        .setParameter("height", height)
                        .getResultList();
                towers.forEach(System.out::println);
            }
        }
        return true;
    }

    public static void main(String[] args) {
        EntityManagerFactory factory = Persistence.createEntityManagerFactory("jpa");
        EntityManager em = factory.createEntityManager();
        var scanner = new Scanner(System.in);
        var isRunning = true;

        addData(em);

        while (isRunning) {
            System.out.println(
                """
                Choose a command:
                0. Exit
                1. List all mages
                2. List all towers
                3. Add a mage
                4. Add a tower
                5. Add a mage to a tower
                6. Remove a mage
                7. Remove a tower
                8. Remove a mage from a tower
                9. Show all mages with level above a certain value
                10. Show all towers with height below a certain value
                """
            );

            int command = Integer.parseInt(scanner.nextLine());

            try {
                isRunning = processCommand(em, scanner, command);
            }
            catch (Exception e) {
                System.out.println("Incorrect input");
                System.out.println(e.getMessage());
            }
        }
        factory.close();
        em.close();
    }
}