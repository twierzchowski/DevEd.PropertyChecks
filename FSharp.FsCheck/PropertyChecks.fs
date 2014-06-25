module FSharp.FsCheck.PropertyChecks

open FsCheck
open NUnit.Framework
open PhoneNumber

type GeneratedValidNumber = 
    { Country : int
      Identifier : int option
      Subscriber : int
      InputString : string }

let validNumberGen = 
    gen { 
        let! c = Gen.choose (1, 999)
        let! i = Gen.oneof [ gen { let! i = Gen.choose (1, 9999)
                                   return (Some i) }
                             gen { return None } ]
        let maxSubLength = 
            float <| 15 - (c.ToString().Length) - (match i with
                                                   | None -> 0
                                                   | Some x -> x.ToString().Length)
        let! s = Gen.choose (1, (int <| 10. ** maxSubLength) - 1)
        return { Country = c
                 Identifier = i
                 Subscriber = s
                 InputString = 
                     sprintf "+%d%s %d" c (match i with
                                           | None -> ""
                                           | Some x -> sprintf " %d" x) s }
    }

type PhoneNumberGenerators =
    static member Valid() =
        { new Arbitrary<GeneratedValidNumber>() with
            override x.Generator = validNumberGen }

[<Test>]
let ``Sanity check``() = 
    match ValidPhoneNumber "+44 1234 123456" with
    | ValidPhoneNumber n -> 
        Assert.AreEqual(n.CountryCode, 44)
        Assert.AreEqual(n.IdentificationCode, 1234)
        Assert.AreEqual(n.SubscriberNumber, 123456)
    | InvalidPhoneNumber _ -> Assert.Fail()

[<Test>]
let ``Insanity check``() = 
    match ValidPhoneNumber "I'm not a phone number" with
    | ValidPhoneNumber n -> Assert.Fail()
    | InvalidPhoneNumber _ -> ()

[<Test>]
let ``Country code less than 4 digits``() = 
    let genNumber (DontSize(cc : uint32)) = 
        match ValidPhoneNumber("+" + cc.ToString() + " 1234 123456") with
        | ValidPhoneNumber n -> Assert.IsTrue(n.CountryCode.ToString().Length < 4)
        | InvalidPhoneNumber _ -> ()
    Check.QuickThrowOnFailure genNumber

[<Test>]
let ``Valid numbers are counted as valid`` () =
    Arb.register<PhoneNumberGenerators> () |> ignore
    Check.VerboseThrowOnFailure (
        fun (v:GeneratedValidNumber) ->
            match ValidPhoneNumber v.InputString with
            | ValidPhoneNumber _ -> true
            | InvalidPhoneNumber _ -> false)
