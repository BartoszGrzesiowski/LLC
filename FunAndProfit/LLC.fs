#if INTERACTIVE
#else
module LLC
#endif
// Copyright (C) 2016 Bartosz Grzesiowski
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

open System

type PostalAddress = 
    {
    Country : string
    City : string
    AddressLine1 : string
    AddressLine2 : string option
    ZipCode : string
    }

type LegalCapacityToPerformActsInLaw = 
    | Lack
    | Limited
    | Full

type NaturalPersonInfo = 
    {
    FirstName: string
    LastName: string
    PESEL: string
    LegalCapacityToPerformActsInLaw : LegalCapacityToPerformActsInLaw
    }



module LLCBusinessName = 
    open System.Text.RegularExpressions

    type T = 
        private
        | Name of string

    let value (Name s) = s

    /// <summary>
    /// Make sure that name contains appropriate designations.
    /// </summary>
    let create (name: string) =
        let name' = name.Trim()
        let pattern = @"\bsp(ółka|\.)\s+z\s(ograniczoną\s+odpowiedzialnością|o\.\s*o\.)"
        if Regex.IsMatch(name', pattern, RegexOptions.IgnoreCase) then
            Some <| Name name
        else None

type LLCDuration = 
    | Indefinite
    | EndDate of DateTime


module LLCShareCapital = 
    // System.Int32.MaxValue of 2,147,483,647 ought to be enough to store share capital,
    // even considering possible devaluation
    type EISInfo =
        {
        AmountOfShares: int
        ShareValue: int
        }

    type T =
        private
        | EqualIndivisibleShares of EISInfo
        // Add discriminator: UnequalShares of USInfo.T
    
    let create amount value =
        if amount <= 0 || value < 50 || amount * value < 5000 then
            None
        else Some (EqualIndivisibleShares {AmountOfShares = amount
                                           ShareValue = value})
     
    let value (EqualIndivisibleShares info) = info

    let getAmountOfCapital (info : EISInfo) : int =
        match info with
        | {AmountOfShares=x; ShareValue=y} -> x * y

type AoAForm =
    | NotarialDeed
    | OnlineForm

type ArticlesOfAssociation =
    {
    Date : DateTime
    Form : AoAForm
    Changes : DateTime list
    }




type CivilLawEntity =
    | NaturalPerson of NaturalPersonInfo
    | LegalPerson of LegalPerson
    | OrganizationalUnit of OrganizationalUnit
    // Full name: organizational units without legal personality to which the
    // applicable laws have granted legal capacity.

and LegalPerson =
    | CapitalCompany of CapitalCompany
    | StateTreasury
    // ...and dozens of others - not considered in this example. I might add them in the near future.

and CapitalCompany =
    | LimitedLiabilityCompany of LLCInfo
    | JointStockCompany

and OrganizationalUnit =
    | Partnerships of Partnership
    | VoluntaryAssociation
    | HousingCommunity
    // ...and others.

and Partnership =
    | RegisteredPartnership
    | ProfessionalPartnership
    | LimitedPartnership
    | LimitedJointStockPartnership


and ManagementBoard =
    private
    | ManagementBoard of CivilLawEntity list

    /// <summary>
    /// Make sure that all members of the Management Board have full legal capacity to perform acts
    /// in law and that the Management Board is not empty.
    /// </summary>
    static member Create (h :: t as members) =
        let members' = members
                       |> List.choose (fun a ->
                                          match a with
                                          | NaturalPerson {LegalCapacityToPerformActsInLaw = Full} as p -> Some p
                                          | _ -> None
                                      )
                       |> List.distinct
        if List.length members' > 0 then
            Some (ManagementBoard members')
        else None

    member x.Value =
        match x with
        | ManagementBoard s -> s

and SupervisoryBoard =
    private
    | SupervisoryBoard of CivilLawEntity list

    /// <summary>
    /// Make sure that all members of the Supervisory Board have full legal capacity to perform acts
    /// in law and that there are no less than 3 distinct members.
    /// </summary>
    static member Create (h :: t as members) =
        let members' = members
                       |> List.choose (fun a ->
                                          match a with
                                          | NaturalPerson {LegalCapacityToPerformActsInLaw = Full} as p -> Some p
                                          | _ -> None
                                      )
                       |> List.distinct
        
        if (List.length members' < 3) then
            None
        else
            Some (SupervisoryBoard members')

    member x.Value =
        match x with
        | SupervisoryBoard s -> s

and AuditCommitee = 
    private
    | AuditCommitee of CivilLawEntity list

    /// <summary>
    /// Makes sure that all members of the Audit Commitee have full legal capacity to perform acts in
    /// law and that there are no less than 3 distinct members.
    /// </summary>
    static member Create (h :: t as members) =
        let members' = members
                       |> List.choose (fun a ->
                                          match a with
                                          | NaturalPerson {LegalCapacityToPerformActsInLaw = Full} as p -> Some p
                                          | _ -> None
                                      )
                       |> List.distinct
        
        if (List.length members' < 3) then
            None
        else
            Some (AuditCommitee members')

    member x.Value =
        match x with
        | AuditCommitee a -> a

and Shareholders =
    private
    | Shareholders of CivilLawEntity list

    static member Create (shareholders : CivilLawEntity list) =
        let shareholders' = List.distinct shareholders
        
        if (List.length shareholders' = 0) then
            None
        else
            Some (Shareholders shareholders')

    member x.Value =
        match x with
        | Shareholders s -> s

    static member IsSingleShareholderLLC (shareholders : CivilLawEntity list) =
        match shareholders with
        | [ LegalPerson (CapitalCompany (LimitedLiabilityCompany info)) ] -> true
        | _ -> false

and Liquidators =
    private
    | Liquidators of CivilLawEntity list

    /// <summary>
    /// Make sure that all liquidators have full legal capacity to perform acts in the law.
    /// </summary>
    static member Create members =
        let members' = members
                       |> List.choose (fun a ->
                                          match a with
                                          | NaturalPerson {LegalCapacityToPerformActsInLaw = Full} as p -> Some p
                                          | _ -> None
                                      )
                       |> List.distinct
        if List.length members' > 0 then
            Some (Liquidators members')
        else None

    member x.Value =
        match x with
        | Liquidators l -> l

and LLCPhase = 
    | InOrganization
    | InOrganizationInLiquidation of date: DateTime * liquidators: Liquidators
    | InOrganizationRestructruring
    | InOrganizationBankrupt
    | AfterRegistration
    | AfterRegistrationLiquidation of date: DateTime * liquidators: Liquidators
    | AfterRegistrationRestructuring
    | AfterRegistrationBankrupt
    // Transformation?


and LLCInfo = 
    {
      Phase : LLCPhase
      BusinessName : LLCBusinessName.T
      Duration : LLCDuration
      FormationDate : DateTime
      RegisteredSeat : PostalAddress
      ShareCapital : LLCShareCapital.T
      ManagementBoard : ManagementBoard option
      SupervisoryBoard : SupervisoryBoard option
      AuditCommitee : AuditCommitee option
      Shareholders : Shareholders
      HoldersOfCommercialPOA: CivilLawEntity list option
      Journal : string option
    }
    
    /// <summary>
    /// Use this member instead of raw BusinessName property (which is hidden from users anyway,
    /// thanks to the signature file). Phase determines how the name is formatted.
    /// </summary>
    member x.FullBusinessName =
        let rawName = LLCBusinessName.value x.BusinessName
        match x.Phase with
        | InOrganization -> sprintf "%s w organizacji" rawName
        | InOrganizationInLiquidation _-> sprintf "%s w organizacji w likwidacji" rawName
        | InOrganizationBankrupt -> 
            sprintf "%s w organizacji w upadłości" rawName
        | InOrganizationRestructruring -> sprintf "%s w organizacji w restrukturyzacji" rawName
        | AfterRegistration -> sprintf "%s" rawName
        | AfterRegistrationLiquidation _ -> sprintf "%s w likwidacji" rawName
        | AfterRegistrationRestructuring -> sprintf "%s w restrukturyzacji" rawName
        | AfterRegistrationBankrupt -> sprintf "%s w upadłości" rawName

/// <summary>
/// Returns a valid limited liability company - before registration, after conclusion of the articles of association
/// </summary>
/// <remarks>
/// Even though it's technically possible to grant the commercial power of attorney before
/// registration, it wouldn't make much sense, therefore HoldersOfCommercialPOA is None by default.
/// </remarks>
let createLLC  (name : LLCBusinessName.T)
               (aoa : ArticlesOfAssociation)
               (duration : LLCDuration)
               (seat : PostalAddress)
               (capital : LLCShareCapital.T)
               (shareholders : Shareholders)
               (managementBoard : ManagementBoard option)
               (supervisoryBoard : SupervisoryBoard option)
               (auditCommitee : AuditCommitee option)
               (journal : string option) =

               let formationDate = aoa.Date
               let duration' = match duration with
                               | EndDate endDate when endDate.Date <= formationDate ->
                                      failwith "Data rozwiązania spółki (przewidziana w umowie) nie może być wcześniejsza, niż data jej zawiązania"
                               | _ -> duration
               // Make sure that the registered seat is located in Poland.
               if seat.Country.Trim().ToUpper() <> "POLSKA" then
                   failwith "Siedziba spółki z o.o. musi znajdować się w Polsce"
               // A limited liability company may not be formed solely by another
               // single-shareholder limited liability company.
               if Shareholders.IsSingleShareholderLLC shareholders.Value then
                   failwith "Spółka z ograniczoną odpowiedzialnością nie może być zawiązana wyłącznie przez inną jednoosobową spółkę
                   z ograniczoną odpowiedzialnością"
               {
                   Phase = InOrganization
                   BusinessName = name
                   Duration = duration'
                   FormationDate = formationDate
                   RegisteredSeat = seat
                   ShareCapital = capital
                   ManagementBoard = managementBoard
                   SupervisoryBoard = supervisoryBoard
                   AuditCommitee = auditCommitee
                   Shareholders = shareholders
                   HoldersOfCommercialPOA = None
                   Journal = journal
               }

let private isArt213COCCViolated (llc : LLCInfo) =
    let info = LLCShareCapital.value llc.ShareCapital
    let capital = LLCShareCapital.getAmountOfCapital info

    let shareholderCount = llc.Shareholders.Value |> List.length

    let bodiesNotAppointed = Option.isNone llc.SupervisoryBoard && Option.isNone llc.AuditCommitee

    capital > 500000 && shareholderCount > 25 && bodiesNotAppointed

/// <summary>
/// If registered after six months after formation -> go into liquidation. Only LLC in organization can be registered.
/// </summary>
let fileForRegistration (registrationDate : DateTime) (llc : LLCInfo) =
    let timeSpan = registrationDate - llc.FormationDate
    let differenceInMonths = (float timeSpan.Days) / (365.25 / 12.0)
    let duration =
        match llc.Duration with
        | EndDate date when date <= registrationDate -> Indefinite
        | x -> x
    
    let phase = if differenceInMonths >= 6.0 then
                    let boardMembers =
                        match llc.ManagementBoard with
                        | Some b -> b
                        | None -> failwith "Brak członków zarządu mogących być likwidatorami"
                    let liquidatorsOpt = Liquidators.Create boardMembers.Value
                    let liquidators =
                        match liquidatorsOpt with
                        | Some l -> l
                        | None -> failwith "Brak członków zarządu mogących być likwidatorami"
                    InOrganizationInLiquidation (registrationDate, liquidators)
                else AfterRegistration

    if isArt213COCCViolated llc then
        failwith "Naruszenie art. 213 § 2 KSH:
kapitał zakładowy przewyższa kwotę 500 000 zł, wspólników jest więcej niż 25, lecz nie ustanowiono rady nadzorczej lub komisji rewizyjnej"

    match llc.Phase with
    | InOrganization -> {llc with Phase = phase; Duration = duration}
    | _ -> llc

let fileForBankruptcy (llc : LLCInfo) =
    match llc.Phase with
    | InOrganization -> {llc with HoldersOfCommercialPOA = None
                                  Phase = InOrganizationBankrupt}
    | InOrganizationInLiquidation _ -> {llc with Phase = InOrganizationBankrupt}
    | AfterRegistration -> {llc with HoldersOfCommercialPOA = None
                                     Phase = AfterRegistrationBankrupt}
    | AfterRegistrationLiquidation _ -> {llc with Phase = AfterRegistrationBankrupt}
    | _ -> llc

let putIntoLiquidation date liquidators (llc : LLCInfo) =

    match llc.Phase with
    | InOrganization -> { llc with Phase = InOrganizationInLiquidation (date, liquidators)}
    | InOrganizationRestructruring -> { llc with Phase = InOrganizationInLiquidation (date, liquidators)}
    | AfterRegistration | AfterRegistrationRestructuring -> { llc with HoldersOfCommercialPOA = None
                                                                       Phase = AfterRegistrationLiquidation (date, liquidators)}
    | _ -> llc

// TODO: implement.
let getLiableEntities (llc : LLCInfo) = ()

let grantCommercialPOA (NaturalPerson info as holderOfCPOA) llc =
    if info.LegalCapacityToPerformActsInLaw <> Full then
        failwith "Naruszenie art. 109[2] § 2 KC: Prokurentem może być osoba fizyczna mająca pełną zdolność do czynności prawnych."

    match llc.Phase with
    | AfterRegistration | AfterRegistrationRestructuring ->
        match llc.HoldersOfCommercialPOA with
        | Some li -> if List.contains holderOfCPOA li then llc
                     else {llc with HoldersOfCommercialPOA = Some (holderOfCPOA :: li)}
        | None -> {llc with HoldersOfCommercialPOA = Some [holderOfCPOA]}
    | _ -> llc

let revokeCommercialPOA ({HoldersOfCommercialPOA=holdersOfCPOA} as llc : LLCInfo) (NaturalPerson info as holderOfCPOA) =
    match holdersOfCPOA with
    | Some entities ->
        let filtered = entities |> List.except [holderOfCPOA]
        { llc with HoldersOfCommercialPOA = if List.length filtered = 0 then None else Some filtered }
    | None -> llc

/// <summary>
/// Determines whether llc is a single shareholder limited liability company.
/// </summary>
let isSingle (llc : LLCInfo) =
    match llc.Shareholders with
    | Shareholders.Shareholders x when List.length x = 1 -> true
    | _ -> false
