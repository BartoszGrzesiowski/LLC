# Model polskiej spółki z ograniczoną odpowiedzialnością
Copyright (C) 2016  Bartosz Grzesiowski

## Zwięzły opis założeń projektu
Autor programu podjął próbę implementacji norm prawnych dotyczących funkcjonowania spółki z ograniczoną odpowiedzialnością
w formie typów danych środowiska CLR oraz predefiniowanych operacji na tych danych. Każdy konkretny byt znany z części prawa korporacyjnego
odnoszącej się do spółki z ograniczoną odpowiedzialnością znajduje odzwierciedlenie w wartości, która jest przyporządkowana do jednego
z typów danych.

Tytułem przykładu: typ o nazwie `Partnership` składa się z 4 dopuszczalnych wartości: `RegisteredPartnership`, `ProfessionalPartnership`,
`LimitedPartnership`, `LimitedJointStockPartnership`. Każda wartość typu `Partnership` przynależy do bardziej pojemnej kategorii
`OrganizationalUnit`, istniejącej równolegle z kategoriami `LegalPerson` oraz `NaturalPerson`, łącznie zaliczanych do typu
`CivilLawEntity`. Tylko wystąpienia (instances) tego najbardziej ogólnego typu mogą tworzyć grupę wspólników spółki z ograniczoną
odpowiedzialnością.

Mając wiedzę o poszczególnych wspólnikach spółki z o.o., można zdefiniować w programie dalsze reguły, które zapewnią poprawność
przeprowadzonych operacji, np. uniemożliwią zawiązanie spółki z o.o. wyłącznie przez inną jednoosobową spółkę z o.o. Jest to
przykład tzw. reguły biznesowej (business rule), która ma zapewnić wewnętrzną spójność modelu.

Dziedzina stosunków korporacyjnych nie zawiera zbyt wiele przykładów bytów, które są niezmienne; w matematyce taką wartością jest
na przykład liczba 9. W przypadku spółki z o.o., zasady jej funkcjonowania determinuje chociażby forma ustrojowa, która jest zmienna
w czasie. Spółka z o.o. w organizacji jest reprezentowana przez zarząd albo pełnomocnika powołanego jednomyślną uchwałą wspólników.
Po otwarciu likwidacji spółę reprezentują likwidatorzy. Dlatego immanentnym elementem systemu informatycznego odzwierciedlającego
ramy prawne działania spółek z o.o. muszą być reguły zapewniające poprawność przemian między formami ustrojowymi spółki. Nie do
pomyślenia jest przecież zarejestrowanie spółki, która jest w upadłości. Może inaczej - taką sytuację można sobie wyobrazić, ale
poprawnie działający system ma za zadanie do tego nie dopuścić.

Nawet jeśli mamy do czynienia tylko z danymi niezmiennymi albo zapewnimy poprawność zmian stanu bytu w czasie, nie możemy dopuścić
do tego, żeby konkretna reprezentacja bytu w systemie zawierała niespójne dane.

Pierwszym krokiem w kierunku osiągnięcia tego celu jest odpowiednie zdefiniowanie dziedziny tych wartości. Przyjmijmy, że właściwością
umowy spółki z o.o. jest data jej zawarcia. Ogólnie rzecz biorąc, każda data to pewien punkt w czasie. Czy jednak mogło dojść do zawarcia
takiej umowy w dacie śmierci Piotra Abelarda (21 kwietnia 1142)? Oczywiście, że nie. Nawet gdyby ktoś w owym czasie skonstruował stosunek
prawny o bardzo podobnej charakterystyce, to i tak stworzony przez autora program nie nadawałby się do analizy stosunków panujących w tej
spółce, ponieważ odnosi się do innego otoczenia prawnego - nie pozwala on chociażby na stworzenie spółki, której siedziba byłaby położona
poza Polską (a w tej dacie nie istniało państwo polskie, panowało rozbicie dzielnicowe i ustrój senioralny). Dlatego bardziej trafne byłoby
wprowadzenie ograniczenia, zgodnie z którym data zawarcia umowy spółki z o.o. nie może być wcześniejsza, niż wejście w życie ustawy,
która umożliwiła tworzenie spółek z o.o. w obecnym kształcie.

Kolejnym krokiem, który zapewni odpowiednie działanie systemu, jest ograniczenie sposobów manipulacji danymi do predefiniowanych operacji,
które implementują logikę walidacyjną. Wartość pewnych właściwości złożonego bytu zależy niekiedy od wartości innych właściwości i nie
mogą być zmieniane w odosobnieniu. Przyjmijmy, że adres spółki z o.o. stanowi uporządkowaną sekwencję ciągów znaków reprezentujących
kolejno: państwo, miejscowość, ulicę oraz kod pocztowy. Czy możemy dopuścić do zmiany miejscowości, bez wzięcia pod uwagę zmian pozostałych
danych adresowych? Zmiana miejscowości pociąga za sobą niekiedy zmianę zarówno państwa, ulicy jak i kodu pocztowego. Jeśli nie uwzględnimy
tych wewnętrznych reguł, dziedzina dopuszczalnych wartości złożonych reprezentujących adresy w naszym modelu będzie stanowiła niejako
prosty iloczyn kartezjański, stanowiący zbiór wszystkich uporządkowanych sekwencji ciągów znaków, choćby konkretne ich zestawienie było
pozbawione sensu z punktu widzenia reguł, które zamierzamy odzwierciedlić w programie. Dopuścimy na przykład do ustanowienia prokury
w spółce w stanie upadłości, czemu powinniśmy zapobiec.

Zdaniem piszącego te słowa, nawet małpa postawiona przed klawiaturą nie powinna wprowadzić logicznie niespójnych
danych do programu. Autor nie dysponuje wszelako małpą, dlatego twierdzenie to może okazać się łatwo falsyfikowalne.

Czemu ma służyć stworzenie spójnego modelu odzwierciedlającego pewien wycinek stosunków społecznych normowanych przez
prawo? Przecież spółki są rejestrowane w KRS, a nie w rejestrach CPU.

Na tak postawione pytanie autor może odpowiedzieć w następujący sposób. Użytkownik poznaje reguły rządzące systemem
informatycznym w wyniku interakcji. Operacja niedozwolona nie zostanie wykonana, dane niespójne nie występują w programie.
Osoba korzystająca z programu zna wszystkie dostępne w nim operacje oraz stany. Ma niezawodną i łatwo dostępną wiedzę o działaniu
systemu, będącego jednakże bardzo ubogą imitacją rzeczywistości. Tymczasem uczestnik obrotu prawnego nie może sobie pozwolić na
żadne eksperymenty. Zrekonstruowanie i zastosowanie w konkretnym przypadku normy prawnej wymaga przeanalizowania wielu przepisów,
przeważnie ulokowanych w różnych aktach normatywnych. Sprawny interpretator tych aktów jest w stanie udzielić odpowiedzi na pytania
zadane przez osobę zainteresowaną działaniem systemu prawnego; być może nawet ma na tyle dobrą pamięć, że potrafi wyrywkowo wskazać
wszystkie znane mu potencjalne interakcje, które mogą wystąpić w tym systemie. Doświadczenie uczy wszelako, że maszyny cyfrowe
z uwagi na swoją nieskończononą cierpliwość, pracowitość i efektywność lepiej wywiązują się z powierzonych ich zadań.

Dlatego właśnie duże nadzieje autor programu pokłada w inicjatywach, takich jak system kognitywny [ROSS](http://www.rossintelligence.com/).
100 współbieżnych wątków (threads) programu wykona zadanie sprawniej, niż 100 paralegals; na dodatek - co musi rozgrzać serca właścicieli
polskich kancelarii - komputer zrobi to taniej, gdyż nie trzeba dla niego nawet parzyć kawy. Ale popadam w dygresję...

## Przykład użycia
Przykładowy scenariusz przedstawiający typowe czynności związane z funkcjonowaniem spółki z o.o. jest zawarty w pliku Scenario.fs
(moduł Scenario), w funkcji main - punkt wejścia (entry point). Plik ten zawiera dość obszerne komentarze opisujące działanie programu.
Definicja programu znajduje się w pliku LLC.fs.

## FAQ
1. Wielkie mi mecyje. Kto by nie potrafił omówić regulacji KSH o spółce z o.o.?

Touché. Na pewno czytelnik potrafi ją omówić w środku nocy. I na wspak. Dlatego też wybrałem ją jako wdzięczny obiekt moich
zainteresowań związanych z DDD (Domain Driven Development). Łatwiej jest dzięki temu osiągnąć porozumienie co do treści reguł
dotyczących funkcjonowania tego podmiotu i wychwycić błędy. Jeżeli pomysł okaże się dobry, być może stworzę bardziej
skomplikowane modele.

2. Czym się inspirowałeś?

Na pomysł stworzenia programu wpadłem, czytając artykuł autorstwa mojego guru w dziedzinie programowania, Scotta Wlaschina, opublikowany
pod [tym adresem](https://fsharpforfunandprofit.com/posts/designing-with-types-representing-states/). Jest w nim opisany bardzo krótki
program magazynowy, śledzący zmiany statusu przesyłki (package tracking system).

4. Wspominasz o znaczeniu wewnętrznej spójności danych, podając przykład adresu spółki. Dlaczego w Twoim programie nie sprawdzasz, czy
wpisany adres rzeczywiście istnieje?

Posłużyłem się tym przykładem, ponieważ jest on bardzo prosty. Nie jest moim celem rywalizacja z operatorami pocztowymi i sprawdzanie
poprawności ulic i kodów pocztowych. Z mojego punktu widzenia miejscowość może się nazywać "Twin Peaks" albo "123". Nie po to napisałem
program w ekspresywnym języku, umieszczając jego definicję w jednym pliku (LLC.fs), żeby umieszczać w nim skomplikowane reguły niezwiązane
z prawem.

5. \*głos programisty\* Typy algebraiczne w F# są kosztowne, nie dbasz o szybkość działania programu - to niekoszerne.

\*plonk!\* Niekoszerne są dla mnie typy wyliczeniowe. Lubię single case discriminated unions, typy reprezentujące Zarząd i Radę Nadzorczą
to w zasadzie glorified, tagged lists - z prywatnym konstruktorem. Na swoją obronę dodam, że zapowiadane zmiany w wersji 4.1 języka F#
obejmują wprowadzenie struct unions, struct tuples i struct records, mogę je wykorzystać przy definicji prostszych typów danych. Brzydotę
rekurencyjnych definicji typów ograniczą mutually referential types and modules.

## Do zrobienia
Podobnie jak Beniowski u Słowackiego, autor nie żywi się blaskiem księżyca, dlatego zabrakło mu czasu na zakodowanie
wielu planowanych funkcji; zdecydował się jednak jak najszybciej udostępnić pierwszą działającą (aczkolwiek
niekompletną) wersję programu.

Poniżej znajduje się zestawienie zadań, których nie udało się do tej pory zrealizować.

Dodatkowe reguły:
* [ ] Zakodowanie reguł dotyczących tworzenia kapitału zakładowego o nierównej wartości udziałów, obrotu oraz podziału udziałów;
* [ ] Urzeczywistnienie norm o spółkach dominujących;
* [ ] Uwzględnienie zasady niepołączalności funkcji i stanowisk;
* [ ] Umożliwienie dokonania podziału, łączenia oraz przekształceń spółek z o.o.

Zadania poboczne:
* [ ] Napisanie testów jednostkowych (XUnit), w tym testów weryfikujących właściwości programu (property based 
testing - FsCheck);
* [ ] Stworzenie klienta CLI w celu ułatwienia interakcji z programem.

## Licencja
Program został udostępniony na licencji AGPL, której oryginalny tekst zawarty jest w pliku agpl.txt, stanowiącym
element repozytorium.

Copyright (C) 2016  Bartosz Grzesiowski

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see [link](http://www.gnu.org/licenses/).