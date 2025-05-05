// PlayerGeneratorService.cs
using EsportsManager.Models;

public static class PlayerGeneratorService
{
    private static readonly Random random = new Random();

    private static readonly string[] internationalNicknames = new string[]
    {
        "King", "Emperor", "Monarch", "President", "Dictator", "Crown", "Ambition", "Flash", "Strike", "Leader", "Hand", 
        "Eagle", "Serpent", "Authority", "Body", "Mind", "Legendary", "Heroic", "Warlord", "Lord", "Duke", "Bullet", "Scepter",
        "Queen", "Empress", "Cruiser", "Battle", "Freak", "Insane", "Killer", "Corrupted", "Corruption", "Golden", "Silver",
        "Platinum", "Star", "Titan", "Shotgun", "Rock", "Paper", "Barrel", "Frog", "Lion", "Goat", "Dog", "Mouse", "Monitor",
        "Gun", "Shield", "Sword", "Spear", "Word", "Katana", "Blade", "Aura", "Sigma", "Beta", "Gum", "Hook", "Caps", "CapsLock", "Shift",
        "Control", "Power", "Engineer", "Axe", "Soldier", "Pyro", "Pyromaniac", "Spy", "Medic", "Doctor", "Jester", "Scout", "River",
        "Sniper", "Devil", "Demon", "Angel", "Tank", "Plane", "Ship", "Destroyer", "Chest", "Bald", "Baldy", "Moehawk", "Controller",
        "Insanity", "Brilliance", "Genious", "Shock", "Shook", "Doom", "Gloom", "Thing", "Fantastic", "Invisible", "KnowMe", "Rocket",
        "Deity", "Soul", "Ghost", "Time", "Example", "Simple", "Exquisite", "Fire", "Ice", "Storm", "Cloud"
    };

    private static readonly Dictionary<string, (string[] firstNames, string[] lastNames, string[] nickPrefixes, string[] nickSuffixes)> NationalityData = new()
    {

        ["Germany"] = (
            new[] { "Hans", "Johannes", "Maximilian", "Lukas", "Timo", "Felix", "Leon", "Paul", "Elias", "Carl" },
            new[] { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann" },
            new[] { "Kaiser", "Blitz", "Panzer", "Fuchs", "Sturm", "Eisen", "Donner", "Falke", "Jäger", "Stahl" },
            new[] { "X", "er", "y", "z", "meister", "inator", "man", "fox", "hawk", "king" }
        ),

        ["Bulgaria"] = (
            new[] { "Georgi", "Ivan", "Dimitar", "Alexander", "Stefan", "Nikolay", "Vasil", "Petar", "Hristo", "Todor" },
            new[] { "Ivanov", "Georgiev", "Dimitrov", "Petrov", "Nikolov", "Hristov", "Stoyanov", "Todorov", "Angelov", "Vasilev", "Vazov", "Markov", "Kirev", "Aleksandrov" },
            new[] { "Balkan", "Tsar", "Vihren", "Pirin", "Rila", "Struma", "Maritsa", "Lev", "Orel", "Zmey", "Knqz", "Imperator", "Vlastelin", "Pomak", "Tank", "Bolyar"  },
            new[] { "ov", "sky", "bg", "cho", "to", "ko", "vitz", "off", "ich", "ar" }
        ),

        ["UK"] = (
            new[] { "John", "James", "Robert", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Albert", "Steven", "Alexander" },
            new[] { "Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson", "Davies", "Robinson", "Wright" },
            new[] { "Brit", "Union", "Bulldog", "Baron", "Thames", "Roundabout", "Scottish", "Whiskey", "Pub", "Mate", "Gentleman", "Royal", "Bentley", "Enigma", "Dreadnought", "AirForce" },
            new[] { "ster", "ish", "UK", "GB", "ENG", "SCO", "WAL", "NIR", "ey", "er" }
        ),

        ["Russia"] = (
            new[] { "Arkadii", "Konstantin", "Aleksandr", "Alexey", "Anastas", "Avgust", "Bogdan", "Boris", "Danil", "Dmitrii", "Emil", "Yevgenyi", "Feodor", "Gavril", "Gennadi", "Gennady", "Gerasim", "German", "Igor", "Ioann", "Ivan", "Semion", "Iosif", "Irakliy", "Isak", "Kazimir", "Kirill", "Kliment", "Lavr", "Lavrentiy", "Lazar", "Leonid", "Lev", "Maxim", "Mikhail", "Mitya", "Nikandr", "Nikita", "Nikolay", "Panteley", "Pavel", "Pyotr", "Roman", "Rurik", "Samuil", "Sava", "Stanislav", "Trifon", "Vladimir", "Valeri", "Vasili", "Viktor", "Vlad", "Yakim", "Yakov", "Yegor", "Yulian", "Yevlogy" },
            new[] { "Shefchenko", "Poroshenko", "Machik", "Mogilevich", "Alekseyevich", "Nikolovich", "Nikolov", "Agapov", "Azarov", "Aleksandrov", "Bolotnikov", "Boyarov", "Budnikov", "Benediktov", "Bukov", "Bulgakov", "Vavilov", "Vetrov", "Vlacic", "Gagarin", "Gorbunov", "Golov", "Druganin", "Demidov", "Dudin", "Yenin", "Yermakov", "Yesikov", "Zharkov", "Zhirov", "Zhurov", "Zhilov", "Zakharin", "Zaburin", "Ivakin", "Ignatkovich", "Ilyin", "Inshov", "Lavrov", "Larin", "Lopatin", "Losevsky", "Lukov", "Leskov", "Lvov", "Lipov", "Maksimov", "Mamonov", "Nazarov", "Noskov", "Norin", "Ozerov", "Pashin", "Potapov", "Primakov" },
            new[] { "Konsomol", "Harasho", "Tovarish", "Volga", "Moskvitch", "Gazka", "Vodka", "Leto", "Pozhaluista", "DoSvidanija", "Bomba", "Kruto", "Opasnii" },
            new[] { "ov", "sky", "ski", "off", "ich", "ar" }
        ),

        ["Spain"] = (
            new[] { "Antonio", "Manuel", "José", "Francisco", "David", "Juan", "Javier", "Daniel", "Carlos", "Alejandro", "Miguel", "Ángel", "Fernando", "Pablo", "Sergio" },
            new[] { "García", "Rodríguez", "González", "Fernández", "López", "Martínez", "Sánchez", "Pérez", "Martín", "Gómez", "Ruiz", "Hernández", "Jiménez", "Díaz", "Moreno" },
            new[] { "Toro", "Flamenco", "Ibérico", "Guitarra", "Paella", "Sangria", "Matador", "Don", "Conquistador", "Barcelona", "Madrid", "Gaudí", "Oliva", "Rojo", "Plaza" },
            new[] { "ez", "ón", "ito", "ín", "illo", "ano", "ero", "ante", "dor", "ino" }
        ),

        ["South Korea"] = (
            new[] { "Min-ho", "Joon", "Tae-hyun", "Seung", "Hyun-woo", "Ji-hoon", "Dae-hyun", "Sung-min", "Young-ho", "Kwang-soo", "Jae-suk", "Ki-bum", "Jun-ho", "Won-bin" },
            new[] { "Kim", "Lee", "Park", "Choi", "Jung", "Kang", "Yoon", "Han", "Shin", "Song" },
            new[] { "Dragon", "Tiger", "Han", "Seoul", "K-pop", "Soju", "Taekwon", "Ginseng", "Bibimbap", "Hallyu", "Samsung", "Hanguk", "BTS", "Kimchi", "Joseon" },
            new[] { "-ah", "-ie", "-ssi", "-nim", "-hyung", "-noona", "-ya", "-kku", "-dong", "-ro" }
        ),

        ["USA"] = (
            new[] { "John", "Michael", "James", "David", "Robert", "William", "Joseph", "Daniel", "Matthew", "Christopher", "Andrew", "Joshua", "Ethan", "Ryan", "Anthony" },
            new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Wilson", "Anderson", "Taylor", "Thomas", "Moore" },
            new[] { "Eagle", "Freedom", "Liberty", "Yankee", "Texan", "Hollywood", "Brooklyn", "Captain", "Patriot", "Cowboy", "Grizzly", "Apple", "Gridiron", "Stars", "Bulldog" },
            new[] { "son", "ton", "er", "ey", "man", "el", "ard", "ter", "ing", "ty" }
        ),

        ["China"] = (
            new[] { "Wei", "Jie", "Lei", "Feng", "Tao", "Jun", "Yang", "Hao", "Xin", "Yuan", "Bo", "Kai", "Jian", "Qiang", "Ming" },
            new[] { "Wang", "Li", "Zhang", "Liu", "Chen", "Yang", "Huang", "Zhao", "Wu", "Zhou", "Xu", "Sun", "Ma", "Zhu", "Hu" },
            new[] { "Dragon", "Phoenix", "GreatWall", "Panda", "Lotus", "Jade", "Ming", "Confucius", "Shaolin", "Forbidden", "Red", "Beijing", "Shanghai", "KungFu", "Tea" },
            new[] { "zi", "er", "long", "wei", "ming", "jun", "bo", "feng", "hao", "sheng" }
        ),

        ["France"] = (
            new[] { "Jean", "Pierre", "Michel", "Thomas", "Nicolas", "François", "David", "Patrick", "Christophe", "Alexandre", "Antoine", "Louis", "Olivier", "Philippe", "Vincent" },
            new[] { "Martin", "Bernard", "Dubois", "Thomas", "Robert", "Richard", "Petit", "Durand", "Leroy", "Moreau", "Simon", "Laurent", "Lefebvre", "Michel", "Garcia" },
            new[] { "Baguette", "Beret", "Château", "Citroën", "Dijon", "Escargot", "Louvre", "Marseille", "Napoleon", "Paris", "Rouge", "Tour", "Versaille", "Vin", "Zidane" },
            new[] { "eau", "el", "et", "on", "in", "ier", "ais", "and", "ot", "ou" }
        ),

        ["Brazil"] = (
            new[] { "João", "Pedro", "Lucas", "Gabriel", "Carlos", "Daniel", "Marcos", "Rafael", "Felipe", "Bruno", "André", "Eduardo", "Leonardo", "Thiago", "Vinicius" },
            new[] { "Silva", "Santos", "Oliveira", "Souza", "Rodrigues", "Ferreira", "Alves", "Pereira", "Lima", "Costa", "Gomes", "Martins", "Ribeiro", "Carvalho", "Monteiro" },
            new[] { "Carnaval", "Samba", "Joga", "Pelé", "Café", "Amazon", "Carioca", "Bossa", "Capoeira", "Favela", "Malandro", "Ouro", "Tropical", "Cristo", "Futebol" },
            new[] { "ão", "inho", "oso", "eiro", "al", "ino", "ito", "ano", "ico", "as" }
        ),

        ["Ukraine"] = (
            new[] { "Oleksandr", "Dmytro", "Andriy", "Serhiy", "Ivan", "Mykhailo", "Volodymyr", "Vitaliy", "Yuriy", "Pavlo", "Bohdan", "Taras", "Ihor", "Viktor", "Roman" },
            new[] { "Melnyk", "Shevchenko", "Kovalenko", "Bondarenko", "Tkachenko", "Kravchenko", "Oliynyk", "Savchenko", "Petrenko", "Klymenko", "Polishchuk", "Lysenko", "Zaytsev", "Hryhorovych", "Sydorenko" },
            new[] { "Cossack", "Bandura", "Chornobyl", "Dnipro", "Kyiv", "Tryzub", "Peremoha", "Hryvnia", "Kozak", "Polissya", "Sich", "Steppe", "Varenyky", "Kalyna", "Holodomor" },
            new[] { "enko", "chuk", "ko", "skyi", "yk", "iv", "ov", "ych", "ak", "ar" }
        ),

        ["Sweden"] = (
            new[] { "Erik", "Lars", "Karl", "Anders", "Johan", "Per", "Nils", "Mikael", "Björn", "Gunnar", "Hans", "Fredrik", "Magnus", "Stefan", "Olof" },
            new[] { "Andersson", "Johansson", "Karlsson", "Nilsson", "Eriksson", "Larsson", "Olsson", "Persson", "Svensson", "Gustafsson", "Pettersson", "Jönsson", "Bengtsson", "Lindberg", "Axelsson" },
            new[] { "Viking", "Volvo", "IKEA", "Fika", "ABBA", "Nordic", "Aurora", "Stockholm", "Midsommar", "Lagom", "Skog", "Björn", "Fjord", "Krona", "Göteborg" },
            new[] { "sson", "berg", "ström", "lund", "gren", "qvist", "blom", "dal", "holm", "näs" }
        ),

        ["Norway"] = (
            new[] { "Ole", "Jan", "Lars", "Bjørn", "Per", "Kjell", "Erik", "Tor", "Svein", "Arne", "Geir", "Morten", "Trond", "Stian", "Anders" },
            new[] { "Hansen", "Johansen", "Olsen", "Larsen", "Andersen", "Pedersen", "Nilsen", "Kristiansen", "Karlsen", "Jensen", "Berg", "Solberg", "Svendsen", "Eriksen", "Haaland" },
            new[] { "Fjord", "Viking", "Troll", "Nordic", "Aurora", "Oslo", "Bergen", "Stavanger", "Lutefisk", "Kroner", "Ski", "Thor", "Edda", "Lofoten", "Frozen" },
            new[] { "sen", "son", "en", "stad", "vik", "fjord", "berg", "dal", "nes", "rud" }
        ),

        ["Netherlands"] = (
            new[] { "Jan", "Piet", "Kees", "Henk", "Willem", "Gerard", "Cornelis", "Dirk", "Johan", "Peter", "Maarten", "Rob", "Bas", "Frank", "Huub" },
            new[] { "de Jong", "Jansen", "de Vries", "van den Berg", "van Dijk", "Bakker", "Janssen", "Visser", "Smit", "Meijer", "de Boer", "Mulder", "de Groot", "Bos", "Vos" },
            new[] { "Tulip", "Windmill", "Orange", "Amsterdam", "Gouda", "Delta", "Canal", "Stroopwafel", "Hague", "Bicycle", "Dyke", "Van", "Delft", "Friesland", "Zeeland" },
            new[] { "sen", "man", "stra", "ma", "se", "ker", "veld", "dam", "hof", "aar" }
        ),

        ["Italy"] = (
            new[] { "Giuseppe", "Antonio", "Mario", "Luigi", "Giovanni", "Francesco", "Marco", "Alessandro", "Paolo", "Stefano", "Roberto", "Andrea", "Riccardo", "Davide", "Simone" },
            new[] { "Rossi", "Russo", "Ferrari", "Esposito", "Bianchi", "Romano", "Colombo", "Ricci", "Marino", "Greco", "Bruno", "Gallo", "Conti", "De Luca", "Mancini" },
            new[] { "Pizza", "Pasta", "Roman", "Venetian", "Dolce", "Vespa", "Ferrari", "Leonardo", "Michelangelo", "Colosseum", "Gondola", "Barista", "Opera", "Tuscany", "Sicilian" },
            new[] { "ini", "etti", "oni", "esco", "ino", "ari", "azzo", "ucci", "elli", "otti" }
        ),

        ["Denmark"] = (
            new[] { "Jens", "Peter", "Michael", "Lars", "Henrik", "Søren", "Christian", "Morten", "Anders", "Thomas", "Jan", "Martin", "Niels", "Ole", "Erik" },
            new[] { "Jensen", "Nielsen", "Hansen", "Pedersen", "Andersen", "Christensen", "Larsen", "Sørensen", "Rasmussen", "Petersen", "Madsen", "Kristensen", "Olsen", "Thomsen", "Møller" },
            new[] { "Viking", "Nordic", "Copenhagen", "Legoland", "Danish", "Hygge", "Mermaid", "Windmill", "Fjord", "Pastry", "Hamlet", "Aarhus", "Odense", "Kroner", "Bicycle" },
            new[] { "sen", "son", "berg", "gaard", "holm", "dahl", "lund", "toft", "by", "rup" }
        ),

        ["Finland"] = (
            new[] { "Jari", "Mika", "Antti", "Pekka", "Juha", "Marko", "Heikki", "Janne", "Timo", "Kari", "Matti", "Sami", "Ari", "Eero", "Jukka" },
            new[] { "Korhonen", "Virtanen", "Mäkinen", "Nieminen", "Mäkelä", "Hämäläinen", "Laine", "Heikkinen", "Koskinen", "Järvinen", "Lehtonen", "Saarinen", "Salminen", "Lahtinen", "Rantanen" },
            new[] { "Sauna", "Nokia", "Sisu", "Aurora", "Lakeland", "Moose", "Rally", "Kalevala", "Helsinki", "Turku", "Tundra", "Sibelius", "AngryBirds", "Kantele", "Salmiakki" },
            new[] { "nen", "inen", "la", "lä", "ki", "to", "ri", "ma", "ja", "ko" }
        ),

        ["Australia"] = (
            new[] { "Jack", "Liam", "Noah", "William", "James", "Lucas", "Benjamin", "Ethan", "Alexander", "Samuel", "Oliver", "Daniel", "Matthew", "Henry", "Thomas" },
            new[] { "Smith", "Jones", "Williams", "Brown", "Wilson", "Taylor", "Johnson", "Anderson", "White", "Martin", "Lee", "Clark", "Thomas", "Roberts", "Walker" },
            new[] { "Outback", "Kangaroo", "Surf", "Boomer", "Aussie", "Koala", "Sydney", "Melbourne", "Brisbane", "Darwin", "Uluru", "Dingo", "Barra", "G'day", "Sheila" },
            new[] { "o", "ie", "y", "er", "ey", "man", "za", "bo", "do", "mo" }
        ),

        ["Poland"] = (
            new[] { "Jan", "Andrzej", "Piotr", "Krzysztof", "Stanisław", "Tomasz", "Paweł", "Marcin", "Michał", "Grzegorz", "Jakub", "Adam", "Łukasz", "Zbigniew", "Dariusz" },
            new[] { "Nowak", "Kowalski", "Wiśniewski", "Dąbrowski", "Lewandowski", "Wójcik", "Kamiński", "Kowalczyk", "Zieliński", "Szymański", "Woźniak", "Kozłowski", "Jankowski", "Mazur", "Wojciechowski" },
            new[] { "Vodka", "Pierogi", "Warsaw", "Krakow", "Solidarity", "Winged", "Zloty", "Baltic", "Silesia", "Polonia", "Kurwa", "Bison", "Wawel", "Gdansk", "Zubrowka" },
            new[] { "ski", "cki", "ak", "ek", "ik", "yk", "owicz", "ak", "arz", "yn" }
        ),

        ["Kazakhstan"] = (
            new[] { "Nur", "Daniyar", "Aslan", "Arman", "Bakhyt", "Ruslan", "Serik", "Aibek", "Kairat", "Zhandos", "Yerlan", "Marat", "Almas", "Azamat", "Eldar" },
            new[] { "Kazakhov", "Ivanov", "Smagulov", "Abdirov", "Nurgaliev", "Tulegenov", "Zhumagaliev", "Suleimenov", "Omarov", "Karimov", "Rakhimov", "Sarsenov", "Baimukhanov", "Yerzhanov", "Tursunov" },
            new[] { "Steppe", "Nomad", "Astana", "Borat", "Eagle", "Kumis", "Shanyrak", "Almaty", "Tenge", "Kazakh", "Baikonur", "Apple", "Golden", "Aul", "Saryarka" },
            new[] { "ov", "ev", "in", "uly", "khan", "bek", "ai", "dar", "kan", "sha" }
        ),

        ["Serbia"] = (
            new[] { "Nikola", "Marko", "Aleksandar", "Stefan", "Milan", "Ivan", "Dusan", "Nenad", "Vladimir", "Petar", "Milos", "Dejan", "Dragan", "Bojan", "Zoran" },
            new[] { "Petrović", "Jovanović", "Nikolić", "Marković", "Đorđević", "Stojanović", "Ilić", "Stanković", "Pavlović", "Milošević", "Radović", "Todorović", "Popović", "Lazić", "Kovačević" },
            new[] { "Rakija", "Slivovitz", "Kosovo", "Belgrade", "Šumadija", "Vojvodina", "Dinar", "Šajkača", "Ćevapi", "Gusle", "Tesla", "Orthodox", "Kolo", "Drina", "Sarma" },
            new[] { "ić", "ović", "ević", "in", "ac", "an", "ić", "ić", "ić", "ić" }
        ),

        ["Romania"] = (
            new[] { "Ion", "Mihai", "Andrei", "Alexandru", "George", "Cristian", "Adrian", "Florin", "Daniel", "Constantin", "Gabriel", "Marius", "Vasile", "Radu", "Bogdan" },
            new[] { "Popa", "Popescu", "Ionescu", "Nistor", "Stoica", "Dumitrescu", "Georgescu", "Stan", "Radu", "Florescu", "Munteanu", "Dobre", "Gheorghe", "Barbu", "Tudor" },
            new[] { "Dracula", "Carpathian", "Bucharest", "Transylvania", "Dacia", "Leu", "Sarmale", "Mămăligă", "Vampire", "Danube", "Bran", "Țuică", "Olt", "Doina", "Hora" },
            new[] { "escu", "eanu", "aru", "oiu", "ache", "aru", "anu", "ila", "uță", "aru" }
        ),

        ["Greece"] = (
            new[] { "Yiannis", "Dimitris", "Giorgos", "Kostas", "Nikos", "Panagiotis", "Vasilis", "Christos", "Stavros", "Thanasis", "Michalis", "Alexandros", "Spyros", "Petros", "Andreas" },
            new[] { "Papadopoulos", "Papadakis", "Ioannidis", "Georgiou", "Dimitriadis", "Nikolaidis", "Pappas", "Antoniou", "Karagiannis", "Vlachos", "Makris", "Theodorou", "Panagiotopoulos", "Raptis", "Sideris" },
            new[] { "Olympus", "Athens", "Sparta", "Feta", "Ouzo", "Zeus", "Aegean", "Sirtaki", "Acropolis", "Hellas", "Moussaka", "Santorini", "Mykonos", "Poseidon", "Parthenon" },
            new[] { "opoulos", "akis", "idis", "iou", "akos", "elis", "anos", "as", "is", "os" }
        ),

        ["Turkey"] = (
            new[] { "Mehmet", "Mustafa", "Ahmet", "Ali", "Hüseyin", "İbrahim", "Osman", "Yusuf", "Murat", "Ömer", "Ramazan", "Emre", "Serkan", "Fatih", "Kemal" },
            new[] { "Yılmaz", "Kaya", "Demir", "Şahin", "Çelik", "Yıldız", "Aydın", "Öztürk", "Arslan", "Taş", "Korkmaz", "Polat", "Koç", "Erdoğan", "Aksoy" },
            new[] { "Kebab", "Baklava", "Istanbul", "Ottoman", "Bosphorus", "Ankara", "Döner", "Crescent", "Sultan", "Hammam", "Minaret", "Tulip", "Carpet", "Raki", "Efes" },
            new[] { "oğlu", "er", "han", "demir", "taş", "kan", "çı", "lı", "man", "ar" }
        ),

        ["Albania"] = (
            new[] { "Gjergj", "Arben", "Agron", "Bashkim", "Besnik", "Dritan", "Edmond", "Fatos", "Ilir", "Kastriot", "Luan", "Naim", "Pjetër", "Shpend", "Valon" },
            new[] { "Hoxha", "Prifti", "Dervishi", "Krasniqi", "Gega", "Bardhi", "Leka", "Meta", "Berisha", "Kadare", "Toska", "Zogu", "Basha", "Kola", "Doda" },
            new[] { "Eagle", "Bunker", "Tirana", "Besa", "Kanun", "Skanderbeg", "Albanian", "Dukagjin", "Iso", "Furgon", "Kosova", "Shqip", "Byrek", "Raki", "Vala" },
            new[] { "aj", "i", "u", "shi", "ku", "ri", "ti", "ni", "ka", "za" }
        ),

        ["Kosovo"] = (
            new[] { "Artan", "Bekim", "Dardan", "Fisnik", "Genc", "Hasan", "Ilir", "Jetmir", "Kastriot", "Liridon", "Naim", "Petrit", "Qendrim", "Shpend", "Valon" },
            new[] { "Krasniqi", "Gashi", "Berisha", "Morina", "Bytyqi", "Thaçi", "Hoti", "Kadriu", "Rexhepi", "Shatri", "Zeka", "Hoxha", "Kurti", "Maliqi", "Pllana" },
            new[] { "Dardania", "Newborn", "Pristina", "Flaka", "Besa", "Kosovar", "Rugova", "Dukagjin", "Peja", "Prizren", "Mitrovica", "Shqip", "Flamuri", "Kulla", "Kosova" },
            new[] { "aj", "i", "u", "qi", "ni", "ri", "ti", "ku", "sha", "za" }
        ),

        ["Mexico"] = (
            new[] { "Juan", "José", "Miguel", "Luis", "Carlos", "Jorge", "Francisco", "Antonio", "Pedro", "Manuel", "Alejandro", "Javier", "Ricardo", "Fernando", "Eduardo" },
            new[] { "Hernández", "García", "Martínez", "López", "González", "Pérez", "Rodríguez", "Sánchez", "Ramírez", "Flores", "Gómez", "Torres", "Díaz", "Vargas", "Cruz" },
            new[] { "Sombrero", "Taco", "Chihuahua", "Frida", "Aztec", "Mariachi", "Guadalajara", "Cancún", "Puebla", "Oaxaca", "Jalapeño", "Cinco", "Piñata", "Mole", "Aguila" },
            new[] { "ez", "es", "o", "os", "an", "in", "as", "el", "ón", "co" }
        ),

        ["Argentina"] = (
            new[] { "Juan", "Carlos", "Javier", "Alejandro", "Martín", "Diego", "Gabriel", "Sergio", "Pablo", "Daniel", "Luis", "Fernando", "Ricardo", "Miguel", "Eduardo" },
            new[] { "González", "Rodríguez", "Gómez", "Fernández", "López", "Díaz", "Martínez", "Pérez", "Sánchez", "Romero", "Sosa", "Alvarez", "Torres", "Ruiz", "Ramírez" },
            new[] { "Tango", "Gaucho", "Pampa", "Evita", "Maradona", "Messi", "Patagonia", "Asado", "Mate", "Malbec", "Che", "Buenos", "Aires", "Cataratas", "Polo" },
            new[] { "ez", "es", "an", "in", "as", "el", "ón", "co", "do", "ta" }
        ),

        ["Colombia"] = (
            new[] { "Juan", "Carlos", "Andrés", "Javier", "Alejandro", "Luis", "David", "Santiago", "Daniel", "Jorge", "Fernando", "Ricardo", "Miguel", "Diego", "Camilo" },
            new[] { "Rodríguez", "Gómez", "González", "Martínez", "López", "Hernández", "Díaz", "Pérez", "Sánchez", "Ramírez", "Torres", "Flórez", "Vargas", "Moreno", "Rojas" },
            new[] { "Café", "Salsa", "Gabriel", "García", "Marquez", "Cartagena", "Bogotá", "Medellín", "Paisa", "Arepa", "Chocó", "Amazonas", "Guajira", "Cumbia", "Sombrero" },
            new[] { "ez", "es", "as", "an", "in", "os", "ón", "co", "do", "ta" }
        ),

        ["South Africa"] = (
            new[] { "Jacob", "Thabo", "Sipho", "Mandla", "Tshepo", "Lungelo", "Siyabonga", "Kgosi", "Nkosinathi", "Vusi", "Bongani", "Themba", "Andile", "Mzwandile", "Sandile" },
            new[] { "Dlamini", "Nkosi", "Mthembu", "Zulu", "Khumalo", "Mbeki", "Mandela", "Tambo", "Buthelezi", "Mabaso", "Ndlovu", "Ngcobo", "Cele", "Mkhize", "Zuma" },
            new[] { "Safari", "Biltong", "Braai", "Bafana", "Springbok", "Kruger", "Table", "Cape", "Apartheid", "Ubuntu", "Protea", "Vuvuzela", "Boer", "Zulu", "Soweto" },
            new[] { "ni", "lo", "ba", "zi", "so", "tha", "nga", "bo", "ma", "za" }
        ),

        ["India"] = (
            new[] { "Aarav", "Vihaan", "Aditya", "Arjun", "Rahul", "Rohan", "Raj", "Aryan", "Dhruv", "Kabir", "Krishna", "Mohit", "Neel", "Pranav", "Surya" },
            new[] { "Patel", "Sharma", "Singh", "Kumar", "Gupta", "Verma", "Reddy", "Mehta", "Choudhary", "Malhotra", "Joshi", "Iyer", "Nair", "Rao", "Agarwal" },
            new[] { "Taj", "Bollywood", "Chai", "Karma", "Namaste", "Guru", "Yoga", "Spice", "Himalaya", "Kerala", "Delhi", "Mumbai", "Bengal", "Sari", "Bharat" },
            new[] { "an", "ar", "esh", "esh", "esh", "esh", "esh", "esh", "esh", "esh" }
        ),

        ["Japan"] = (
            new[] { "Haruto", "Riku", "Yuto", "Sota", "Ren", "Yuki", "Kaito", "Haru", "Sora", "Daiki", "Kai", "Hinata", "Ryota", "Takumi", "Shota" },
            new[] { "Sato", "Suzuki", "Takahashi", "Tanaka", "Watanabe", "Ito", "Yamamoto", "Nakamura", "Kobayashi", "Kato", "Yoshida", "Yamada", "Sasaki", "Yamaguchi", "Matsumoto" },
            new[] { "Samurai", "Ninja", "Sakura", "Fuji", "Sushi", "Anime", "Shogun", "Tokyo", "Kyoto", "Daimyo", "Bushido", "Kawaii", "Oni", "Sumo", "Katana" },
            new[] { "to", "ki", "ru", "ta", "shi", "ya", "ma", "ro", "n", "su" }
        ),
        ["Mongolia"] = (
            new[] { "Bat", "Bold", "Tuvshin", "Ganbaatar", "Otgonbayar", "Altan", "Chuluun", "Naran", "Sarnai", "Temujin", "Khasar", "Subotai", "Jamukha" },
            new[] { "Khan", "Tsogt", "Bataar", "Dorj", "Nergui", "Altankhuyag", "Bayar", "Munkh", "Gantulga", "Enkh" },
            new[] { "Steppe", "Ger", "Horse", "Eagle", "Nomad", "Gobi", "Khanate", "BlueSky", "Wolf", "Hunnu", "SilkRoad", "Altai", "Yurt" },
            new[] { "MN", "MGL", "ia", "iin", "tai", "chuud", "khan", "jin", "gar", "uu" }
        ),
        ["Philippines"] = (
            new[] { "Juan", "Jose", "Antonio", "Manuel", "Fernando", "Carlos", "Ramon", "Ricardo", "Eduardo", "Luis", "Miguel", "Alfredo", "Rodrigo" },
            new[] { "Santos", "Reyes", "Cruz", "Bautista", "Garcia", "Aquino", "Dela Cruz", "Gonzales", "Ramos", "Torres" },
            new[] { "Bahay", "Barangay", "Jeepney", "Adobo", "Mango", "Volcano", "Island", "Monsoon", "Pearl", "Bayan", "Maharlika", "Luzon", "Visayas" },
            new[] { "PH", "Pinas", "ng", "ko", "han", "tay", "sil", "non", "dito", "royo" }
        ),
        ["Vietnam"] = (
            new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh", "Phan", "Vu", "Dang", "Bui", "Do", "Ho", "Ngo" },
            new[] { "Van", "Thi", "Hong", "Minh", "Quang", "Duc", "Huu", "Thanh", "Cong", "Tuan" },
            new[] { "Dragon", "Lotus", "Pho", "Ao Dai", "Cyclo", "Rice", "Bamboo", "Tet", "Saigon", "Halong", "Trung", "Bia Hoi", "Conical" },
            new[] { "VN", "Viet", "Nam", "anh", "inh", "ong", "ac", "ai", "em", "ieu" }
        ),
        ["Indonesia"] = (
            new[] { "Budi", "Agus", "Dwi", "Eko", "Hadi", "Joko", "Tri", "Ahmad", "Surya", "Rudi", "Adi", "Hendra", "Yanto" },
            new[] { "Santoso", "Wijaya", "Pratama", "Setiawan", "Kurniawan", "Siregar", "Hidayat", "Saputra", "Gunawan", "Irawan" },
            new[] { "Wayang", "Batik", "Borobudur", "Komodo", "Tempe", "Angklung", "Jamu", "Satay", "Orangutan", "Kecak", "Gamelan", "Merapi", "Nusantara" },
            new[] { "ID", "Indo", "nesia", "wan", "to", "man", "tar", "ja", "di", "san" }
        )
        //[""] = (
        //    new[] { "", },
        //    new[] { "" },
        //    new[] { "" },
        //    new[] { "" }
        //),
        // Add more nationalities here...
    };

    public static Player GeneratePlayer(string nationality)
    {
        if (!NationalityData.ContainsKey(nationality))
            nationality = "UK"; // Default to UK if nationality not found

        var data = NationalityData[nationality];

        string firstName = data.firstNames[random.Next(data.firstNames.Length)];
        string lastName = data.lastNames[random.Next(data.lastNames.Length)];

        // Generate nickname - multiple patterns
        string nickname;
        int pattern = random.Next(100);

        if(pattern >= 0 && pattern < 30)
        {
            if(pattern >=0 && pattern <=10)
            {
                nickname = AlterCapitalization($"{firstName}{lastName[0]}");
            }
            else if(pattern >= 10 && pattern < 20)
            {
                nickname = AlterCapitalization($"{firstName[0]}{lastName}");
            }
            else
            {
                if(firstName.Length<=5)
                {
                    if (lastName.Length <= 3)
                    {
                        nickname = AlterCapitalization($"{firstName}{lastName}");
                    }
                    else
                    {
                        nickname = AlterCapitalization($"{firstName}{lastName.Substring(0, 3)}");
                    }
                }
                else if(lastName.Length<=5)
                {
                    nickname = AlterCapitalization($"{firstName.Substring(0,3)}{lastName}");
                }
                else
                {
                    nickname = $"{firstName.Substring(0, 3)}{lastName.Substring(0, 3)}";
                }
            }
        }
        else if(pattern >=30 && pattern < 50)
        {
            string prefix = data.nickPrefixes[random.Next(data.nickPrefixes.Length)];
            string suffix = data.nickSuffixes[random.Next(data.nickSuffixes.Length)];
            nickname = $"{prefix}{suffix}";
            // Randomly alter capitalization
            if (random.Next(2) == 0)
                nickname = AlterCapitalization(nickname);
        }
        else if(pattern >= 50 && pattern < 60)
        {
            string prefix = data.nickPrefixes[random.Next(data.nickPrefixes.Length)];
            string suffix = data.nickSuffixes[random.Next(data.nickSuffixes.Length)];
            nickname = $"{prefix}{suffix}";
            // Randomly alter capitalization
            if (random.Next(2) == 0)
                nickname = AlterCapitalization(nickname);
        }
        //else if(pattern >=10 && pattern < 20)
        //{

        //}
        else
        {
            nickname = $"{internationalNicknames[random.Next(internationalNicknames.Length)]}";
            // Randomly alter capitalization, те така за здраве.
            if (random.Next(2) == 0)
                nickname = AlterCapitalization(nickname);
        }
        // Random age between 16 and 30
        int age = random.Next(16, 31);
        DateTime birthDate = DateTime.Now.AddYears(-age).AddMonths(-random.Next(12));

        // Generate stats
        var stats = new PlayerStats
        {
            Aim = random.Next(50, 96),
            Reflexes = random.Next(50, 96),
            GameSense = random.Next(50, 96),
            Teamwork = random.Next(50, 96),
            Consistency = random.Next(50, 96),
            Potential = random.Next(60, 101)
        };
        stats.Rating = CalculateOverallRating(stats);

        return new Player
        {
            FirstName = firstName,
            LastName = lastName,
            Nickname = nickname,
            Nationality = nationality,
            Age = age,
            BirthDate = birthDate,
            Role = (PlayerRole)random.Next(Enum.GetValues(typeof(PlayerRole)).Length),
            Stats = stats,
            MarketValue = CalculateMarketValue(stats, age),
            Salary = CalculateSalary(stats, age)
        };
    }

    private static string AlterCapitalization(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        string sym = "";
        // Randomly change some letters to uppercase
        char[] chars = input.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (random.Next(4) == 0) // 25% chance to change case
            {
                chars[i] = char.IsUpper(chars[i]) ? char.ToLower(chars[i]) : char.ToUpper(chars[i]);
            }
            //e = 3, i = 1, o = 0, a = 4, t = 7
            switch (sym = chars[i].ToString().ToLower())
            {
                case "e":
                    if (random.Next(13) == 0) // 25% chance to change case
                    {
                        chars[i] = '3';
                    }
                    break;
                case "i":
                    if (random.Next(30) == 0) // 25% chance to change case
                    {
                        chars[i] = '1';
                    }
                    break;
                case "o":
                    if (random.Next(8) == 0) // 25% chance to change case
                    {
                        chars[i] = '0';
                    }
                    break;
                case "t":
                    if (random.Next(50) == 0) // 25% chance to change case
                    {
                        chars[i] = '7';
                    }
                    break;
            }
        }
        return new string(chars);
    }

    private static double CalculateOverallRating(PlayerStats stats)
    {
        // Weighted average of stats
        return (stats.Aim * 0.3 + stats.Reflexes * 0.25 + stats.GameSense * 0.2 +
               stats.Teamwork * 0.15 + stats.Consistency * 0.1) / 100.0 * 2.0;
    }

    private static decimal CalculateMarketValue(PlayerStats stats, int age)
    {
        double potentialFactor = stats.Potential / 100.0;
        double ageFactor = 1.0 - Math.Abs(age - 22) * 0.05; // Peaks at 22
        return (decimal)(stats.Rating * 100000 * potentialFactor * ageFactor);
    }

    private static decimal CalculateSalary(PlayerStats stats, int age)
    {
        return CalculateMarketValue(stats, age) / 20m; // Salary is 5% of market value
    }
}