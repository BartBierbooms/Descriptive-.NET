# Descriptive-.NET
Programm in a declarative way  scenario's of an appliccation. Divide definition from execution and implementtion.
# Descriptive-.NET
Programm in a declarative way  scenario's of an appliccation. Divide definition from execution and implementtion.
# Which problem to solve?
Object Oriented programming assumes that a class, its state and methods are tightly united and can be reused. If we follow an analogy in grammar, grammatical speaking this could be true if a class is a subject, the centre of all. But in many cases a class is a direct object in a very specific scenario. You don't want to put these scenario specifics on the class itself. So where do you put them? In many framework that's the task of an Application class, an Orchestrator of whatever. Unfortunately, we do not have a language in how to code scenario's. Implementations that are direct objects in a scenario are scattered around in the whole code base, database inclusive.
# What is needed?
First of all we need to 'separate' scenario from implementation details, just like the Cucumber Given-And-When-Then style of programming. Further I wanted to split up the scenario definions from its execution and i wanted to be able to nest scenario's in scenario's and to extend them. To give an example of the last: You define a scenario. Next you use this scenario and add (extends) an assert scenario to it for testing. Your testing is using the same code as the real application.
# How this is implemented?
This is implemented using a Pipe. You pipe methods in which the output of the current method is the input for the next method. The piping methods themselves are called Then, Iff,TransForm etc., with which you can compose complete scenario's. The complete piping is build around exactly one delegate: `delegate IValueAndSupplement<TV, TS> ToValueSupplementValue<in TI, out TV, out TS>(TI i);`. This delegate is so powerfull that it deserves a separate article.
# Explore
Explore the code base by looking at the Test and Example project.
