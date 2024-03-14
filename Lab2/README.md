# Multithreading Computations

This project is aimed at familiarizing with basic mechanisms of thread creation and synchronization. It is implemented using Apache Maven and Java 11 or higher.

## Task Description

Required to implement a console-based application for parallel execution of complex computations (e.g., naive method for checking primality of large numbers). The complexity of computations can be simulated using the Thread.sleep method for less time-consuming calculations.

### Project Tasks

1. **Task Queue Resource**: Implement a shared resource for submitting tasks. It should allow submitting and retrieving tasks (e.g., prime numbers to be checked). Adding a new task should not remove the previous one if it has not been retrieved yet. The resource should utilize the wait-notify mechanism for thread synchronization.

2. **Result Collection Resource**: Prepare a shared resource for collecting computation results. New results should be added without removing the previous ones. The resource should utilize critical section mechanisms.

3. **Computation Threads**: Implement a thread execution process for the defined computation, which can be passed to a thread instance. Tasks for execution should be continuously retrieved from the appropriate shared resource. Computation results should be saved in the corresponding shared resource.

4. **Application Launch**: Upon starting the application, the required number of threads for executing complex computations should be launched. The number of threads depends on the application's startup parameter. Users should be able to submit new tasks (e.g., by entering a command on the console using System.in and Scanner class).

5. **Graceful Application Shutdown**: Users should have the ability to close the application (e.g., by entering a command on the console using System.in and Scanner class). When the application terminates, all threads should be properly terminated.

## How to Run

1. Clone this repository.
2. Navigate to the project directory.
3. Make sure you have Java 11 or higher and Apache Maven installed.
4. Compile the project using Maven: `mvn compile`
5. Run the project: `mvn exec:java -Dexec.mainClass="org.example.Main"`.

## Contributors

- [R-Ohman](https://github.com/R-Ohman)

Feel free to contribute by forking the repository and submitting pull requests.
