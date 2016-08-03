module Scenario

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

open LLC

// Początek programu.
[<EntryPoint>]
let main _ =
    // Rozpocznijmy od scenariusza, w którym wszystkie wprowadzone dane oraz przeprowadzone operacje są poprawne.
    printfn "Scenariusz 1:"
    printfn "Konstruowanie firmy spółki z ograniczoną odpowiedzialnością..."
    // Firma spółki z o.o. - w tym przypadku ABC spółka z ograniczoną odpowiedzialnością.
    let name =
        let nameOpt = LLCBusinessName.create("ABC Spółka z ograniczoną odpowiedzialnością")
        match nameOpt with
        | Some n -> n
        | None -> failwith "Nieprawidłowa nazwa spółki z o.o."

    // Data zawiązania spółki: dzień uruchomienia programu. Umowa zawarta w formie aktu notarialnego (nie przez Internet).
    let articlesOfassociation =
        {
        Date = DateTime.Today
        Form = NotarialDeed
        Changes = []
        }

    // Czas trwania spółki - nieoznaczony.
    let endDate = Indefinite

    // Siedziba i adres spółki.
    let address =
        {
        Country = "Polska"
        City = "Warszawa"
        AddressLine1 = "Marszałkowska 1"
        AddressLine2 = None
        ZipCode = "00-624"
        }

    // Dane wspólników - wartości shareholder{1-3}. Wspólnikami są 3 osoby fizyczne.
    let shareholder1 =
        NaturalPerson {
                      FirstName = "Janusz"
                      LastName = "Palestry"
                      PESEL = "82030101822"
                      LegalCapacityToPerformActsInLaw = Full
                      }

    let shareholder2 =
        NaturalPerson {
                      FirstName = "Paweł"
                      LastName = "Kowalski"
                      PESEL = "78210209283"
                      LegalCapacityToPerformActsInLaw = Limited
                      }

    let shareholder3 =
        NaturalPerson {
                      FirstName = "Tadeusz"
                      LastName = "Nowak"
                      PESEL = "73220308732"
                      LegalCapacityToPerformActsInLaw = Full
                      }

    printfn "Zrzeszanie wspólników..."
    let shareholders =
        let shareholdersOpt = Shareholders.Create [shareholder1;
                                                   shareholder2;
                                                   shareholder3]
        match shareholdersOpt with
        | Some s -> s
        | None -> failwith "Nieprawidłowe grono wspólników"

    printfn "Tworzenie kapitału zakładowego..."
    // Kapitał zakładowy: 100 udziałów o wartości 200 złotych każdy.
    let capital =
        let shareInfo = LLCShareCapital.create 100 200
        match shareInfo with
        | Some i -> i
        | None -> failwith "Nieprawidłowa wartość i liczba udziałów w kapitale zakładowym"

    // Dane członków Zarządu: boardMember{1-2}. W zarządzie zasiada również jeden wspólnik - shareholder1.
    let boardMember1 =
        NaturalPerson {
                      FirstName = "Adam"
                      LastName = "Żak"
                      PESEL = "6722030776432"
                      LegalCapacityToPerformActsInLaw = Full
                      }

    let boardMember2 =
        NaturalPerson {
                      FirstName = "Krzysztof"
                      LastName = "Przykładny"
                      PESEL = "5921090393752"
                      LegalCapacityToPerformActsInLaw = Full
                      }

    printfn "Powołanie Zarządu..."
    let boardOpt = ManagementBoard.Create [boardMember1;
                                           boardMember2;
                                           shareholder1]

    let board =
        match boardOpt with
        | Some b -> b
        | None -> failwith "Nieprawidłowy skład Zarządu"

    printfn "Zawiązanie spółki..."
    // W spółce nie została ustanowiona Rada Nadzorcza ani Komisja Rewizyjna.
    // W wyniku prawidłowego wywołania funkcji createLLC powstaje spółka z ograniczoną odpowiedzialnością w organizacji - wartość llc.
    // Nieprawidłowe wywołanie skutkuje tzw. wyjątkiem (Exception).
    let llc = createLLC name
                        articlesOfassociation
                        endDate
                        address
                        capital
                        shareholders
                        (Some board)
                        None // Brak Rady Nadzorczej
                        None // Brak Komisji Rewizyjnej
                        None // Nie wskazano pisma przeznaczonego do ogłoszeń spółki.

    // Parametry nazwane (named parameters) poprawiłyby czytelność tego przykładu, ale w F# można ich używać tylko w przypadku członków
    // (members) typu. Wykorzystałem komentarze.

    // W konsoli wyświetla się: "Firma spółki: ABC Spółka z ograniczoną odpowiedzialnością w organizacji"
    printfn "Firma spółki: %s" llc.FullBusinessName

    // W konsoli wyświetla się: "Czy spółka jest jednoosobowa? - Nie"
    let isSingleShareholder = isSingle llc
    printfn "Czy spółka jest jednoosobowa? - %s" (if isSingleShareholder then "Tak" else "Nie")

    let c = LLCShareCapital.value llc.ShareCapital
    // W konsoli wyświetla się: "Kapitał zakładowy spółki wynosi: 20000 zł"
    printfn "Kapitał zakładowy spółki wynosi: %d zł" (LLCShareCapital.getAmountOfCapital c)

    printfn "Rejestrowanie spółki..."
    // Po 20 dniach od zawiązania, spółka zostaje zarejestrowana.
    let llcAfterRegistration = fileForRegistration (articlesOfassociation.Date.AddDays(20.0)) llc

    // W konsoli wyświetla się: "Firma spółki: ABC Spółka z ograniczoną odpowiedzialnością"
    printfn "Firma spółki: %s" llcAfterRegistration.FullBusinessName

    // Zostaje ustanowiona prokura. Prokurent istnieje w pamięci komputera jako wartość holderOfCPOA.
    let holderOfCPOA =
        NaturalPerson {
                      FirstName = "Piotr"
                      LastName = "Prokurencki"
                      PESEL = "90091187332"
                      LegalCapacityToPerformActsInLaw = Full
                      }

    printfn "Ustanawianie prokury..."
    let llcAfterRegistration' = grantCommercialPOA holderOfCPOA llcAfterRegistration

    let CPOAholdersCount =
        match llcAfterRegistration'.HoldersOfCommercialPOA with
        | Some li -> List.length li
        | None -> 0

    // W konsoli wyświetla się: "Liczba prokurentów: 1"
    printfn "Liczba prokurentów: %d" CPOAholdersCount

    printfn "Następuje ogłoszenie upadłości..."
    // Po krótkotwałym okresie burzliwego rozwoju, spółka popada w tarapaty finansowe :-(
    let bankrupt = fileForBankruptcy llcAfterRegistration'

    // W konsoli wyświetla się: "Firma spółki: ABC Spółka z ograniczoną odpowiedzialnością w upadłości"
    printfn "Firma spółki: %s" bankrupt.FullBusinessName

    printfn ""
    printfn "Gotowe."
    0 // Gotowe.
