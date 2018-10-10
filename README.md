# Descriptive-.NET (version 2.0)
Roughly speaking, in OO the object has a state and methods working on that state. For instance, you have a class Payment and a method HasPositiveBalance. The Payment, the transaction involved, the account balance, it is all encompasses in the Payment class.
In real life the playing ground is more complicated, because Payment can operate in a lot of different contexts. For example  in payment clearing, or in payment compliance or in complicated scenario where the payment tax relation is important or where currencies and exchanges are important or in a bookkeeping context with its rounding off problems. You cannot make a Payment class that encompass all the payments parts of these contexts. There is a growing believe that you should not even strive to make such a class. Reuse is perfect for 'technical' classes such as DataTable, or HttpContext, but it won't work for real life business classes like Payment.
Payment gets a meaning because it is acting in a specific context, for instance compliance  where the ultimate beneficial owner should be known. And payment and compliance can act in yet another context, for instance a criminal investigation etc..
To put it more generally, how do we cope with a situation where a specific functionality gets its meaning by the context where it lives in. One object 'acts' upon the functionality of the other. Compliance wants to know the ultimate beneficial owner, and payment must have its role for its discovering. This discovering, this scenario specific interaction is what our business is all about. I think we can solve this problem by using a pipeline and we should give it a far more central place in our design. Before we dive into the solution, you have to keep in mind the conceptional differences of a pipeline versus an OO class.
Conceptual a pipeline construct differs from an OO class construction in various aspects. When you call a method on a class, the class already exists and has an unknown state to the caller. What is returned is up to the implementer of the method. He can return you the return value, throw an exception or returns null. Because the state of the instance of the class is not known, two subsequent calls with the same parameters can result in different return values.
In a pipeline, the outcome of one pipe segment is the input for the next segment. Null values cannot be piped, exception should be handled as a part of the returned values passed through the pipe. The returned value of a pipe should always be predictable. No side effects or state should influence this outcome.
In a pipeline you define subsequent calls. By looking at the chained calls you can reason about its behavior even without knowing what is actually the input of the pipeline. Why not taking this a step further and split definition from execution by design? The pipeline are chained functions to be called. The execution is invoking the pipeline by supplying the data that goes into the pipe. The deterministic outcome is a huge benefit. Think about the pipeline as a formula. The formula itself matters.

Back to our original problem. We want to create a pipeline, where definition is split from execution and where two classes can interact with each other and return a specific scenario bound end result. Surprise, all we need is the following delegate:
```
public delegate IValueAndSupplement<TV, TS> ToValueSupplementValue<in TI, out TV, out TS>(TI i);
```
A delegate is a function pointer (= definition).  The ToValueSupplementValue delegate takes type TI as an input and  instances of class of type TV and TS as an output. You can wrap the latter into an interface:
```
public interface IValueAndSupplement<out TV, out TS>
    {
        TV Val { get; }
        TS SupplementVal { get; }
    }
```

If a have a class ValueAndSupplement that implements that interface I can write the first method:
```
public static ToValueSupplementValue<TI, TI, TV> Init<TI, TV>(Func<TI, TV> init)
            where TV : new
            where TI : new()
        {
            return i => new ValueAndSupplement(I, init(i));
         }
};
```
My pipe start will be:
```
var pipeDefine = Pipe.Init<Payment, Compliance>(_ => new Compliance())
var pipeExecute = pipeDefine(new Payment());
```
If I make a method like (there are many flavors):
```
public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<TV, TS> apply){
     [for now I leave out the implementation]
}
```
You have all elements to construct a pipe with endless Then-segments. This is, because source and returned value are of the same type, namely ToValueSupplementValue, so the returned value can be the source for a new Then pipe segment. You can extend the pipe with as many Then's you need.
```
void GetAccountHolder(Payment p, Compliance c)
{
     cc.FirstBeneficialOwner = p.Account;
}
var pipeDefine = Pipe
                 .Init<Payment, Compliance>(_ => new Compliance())
                 .Then(GetAccountHolder);

var pipeExecute = pipeDefine(PaymentStore.GetPayment(1234));
```
The ToValueSupplementValue out value is an interface. Any class that implements that interface can operate in the Init, Then  pipe methods. The whole pipe will compile without knowing which concrete out object is returned during execution. 

In the default implementation an Option object is returned during execution. Option is an abstract class which, among others,  have the following concrete implementations: None, Some and SomeException. With this construction the pipe returns None when the execution of a pipe segment returns null, and SomeException when this execution ends up in an thrown exception. Because they are all specific implementations of Option, that implements  the IValueAndSupplement interface, the Pipe can still continuing executing and therefore is always executed fully. That means that the above Then implementation goes along the following implementation lines:
```
public static ToValueSupplementValue<TI, TS, TV> Then<TI, TV, TS>(this ToValueSupplementValue<TI, TV, TS> source,
            Action<TV, TS> apply){

     [if(source is None)
         return None;
     if(source is SomeException)
         return SomeException;]

     //Only execute Action in case of Source is Some.
     [real Action implementation]
}
```
When a pipe segment gets a None or SomeException as its input, it will immediately return the None or SomeException. It will skip any implementation defined in the action delegate Apply, which is essentially what you want the Then to perform. In other words the None and SomeException 'stops' any further processing of the pipe.

# Version 2. Possibility to override the default Option return type.
With this version, you can write overwrite the default Option base implementation. As an example i made a separate project with a Validator implementation through a validation class. In stead of an Option, a Validation is returned, which combines validation results or exceptions and a real object representing the business result of a successful pipe execution. This perfectly fits with a Web service situation, where you want to validate your input and execute some logic, but you don't want Exceptions to bubble up to the caller in their original form. Validation errors have a direct impact on the returned response. There can be multiple sources that can return validation results. For the Validation i have added Join and JoinIfValid, which internally are Then-implementations. Join and JoinIfValid specify in a better way the purpose of a Validate pipeline.
# Version 2.0.2. Join
Added convenience Join method to join two pipesegments with the same output type.
```
    var hondaSegment = Pipe.Init<Car, Dealer>(_ => new Dealer())
         .Then(c => c.SetMark(Car.HondaMark))
         .Then(d => d.Reputation = Dealer.eReputation.Bad)
          .Then(d => d.Name = dealerName);

    var hondaDealerSegment = Pipe.Init<Car, Dealer>(() => dealer)
         .Then(c => c.DriveFast());

    var joinedPipe = hondaSegment
         .Then(carDealer => dealer = carDealer.SupplementVal)
        .Join(hondaDealerSegment);

    var pipelineResult = (Option<Car, Dealer>)joinedPipe(new Car());
```
Pipe segments can be tested individually, making TDD an easy option.