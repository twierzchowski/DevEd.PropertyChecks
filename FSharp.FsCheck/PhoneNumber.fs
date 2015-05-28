module FSharp.FsCheck.PhoneNumber

open System.Text.RegularExpressions

type PossibleNumber = 
    { CountryCode : int
      IdentificationCode : int
      SubscriberNumber : int }
     
type PhoneNumber =
    | ValidPhoneNumber of PossibleNumber
    | InvalidPhoneNumber of string

// Shadow the name so that no one else
// can create "ValidPhoneNumber"
let ValidPhoneNumber input =
    let reg = Regex(@"\+(?<cc>\d{1,3}) (?<ic>\d+)\s*(?<sn>\d+)")
    match reg.IsMatch(input) with
    | true ->
        let groups = reg.Match(input).Groups
        ValidPhoneNumber {
            CountryCode = groups.["cc"].Value |> int
            IdentificationCode = groups.["ic"].Value |> int
            SubscriberNumber = groups.["sn"].Value |> int
        }
    | false ->
        InvalidPhoneNumber "No good"
        