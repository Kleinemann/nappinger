using Godot;
using Godot.Collections;
using System.Net.NetworkInformation;

public partial class Player : Animal
{
    static Array<string> NameList = new Array<string>() { "Abod", "Adalbeort", "Adalgar", "Adham", "Adken", "Adulfuns", "Aelf", "Aelfraid", "Aelfric", "Aelor", "Aescby", "Aethel", "Aethelberht", "Aethelisdun", "Ahanor", "Aherne", "Ahrin", "Aidan", "Aidtun", "Aifrid", "Ailean", "Aimil", "Aineislis", "Arileas", "Aislinn", "Alain", "Albhaois", "Albion", "Aldus", "Aler", "Algonthir", "Alraed", "Alhric", "Alhwin", "Alian", "Allsun", "Alviss", "Amalasand", "Amalien", "Amario", "Amber", "Amhiunn", "Amhlaidh", "Amires", "Amlauril", "Amon", "Anant", "Anaurathiel", "Andariel", "Andarius", "Anfalas", "Anhlaoigh", "Anntoin", "Anwyl", "Aodh", "Aodha", "Aodhagan", "Aodhan", "Aoidh", "Aoiffe", "Aonghus", "Aralian", "Aralt", "Arela", "Arheyu", "Arndell", "Arnhold", "Arni", "Arnwald", "Arnwulf", "Arombolosch", "Arregaithel", "Artair", "Arthwr", "Arthylomis", "Artur", "Asgault", "Athàlùsa", "Athdara", "Athdara", "Attewelle", "Avis", "Awurin", "Aylen", "Baehloew", "Bagon", "Bain", "Bairghith", "Baldmar", "Banain", "Banbrigge", "Bangan", "Banlòr", "Banurr", "Bardawulf", "Bardhardt", "Bargash", "Barghan", "Barthr", "Beadu", "Beagan", "Bearach", "Beathag", "Bebhinn", "Becere", "Beledene", "Beonetleah", "Beorc", "Beordtraed", "Beorht", "Beorhthram", "Beormann", "Beornet", "Beorttun", "Beorwalt", "Berchtwald", "Bercleah", "Berdine", "Berin", "Berinhardt", "Bhaird", "Bhaltair", "Bhaltair", "Bhragas", "Binge", "Binok", "Binokee", "Blaecleah", "Blaed", "Blar", "Bliths", "Bloddwyn", "Blotsm", "Bluainach", "Boda", "Bofind", "Bofind", "Bogohardt", "Boltar", "Born", "Boron", "Bothi", "Boyne", "Bradach", "Brangwen", "Brann", "Breandan", "Bret", "Brian", "Bridhid", "Brock", "Bronwyn", "Broth", "Bryn", "Brys", "Buadhach", "Buidhe", "Burgal", "Burr", "Cadawig", "Caddrairc", "Cadel", "Cadhla", "Caellach", "Caerau", "Caerghallan", "Cai", "Cailean", "Caileass", "Cain", "Caitlin", "Calldwr", "Cambeul", "Cameron", "Canshron", "Cant", "Caoinleain", "Caolabhuinn", "Caolaidhe", "Caomh", "Caomhan", "Caomhiun", "Caradoc", "Caramichil", "Cariadland", "Carleas", "Carriag", "Carridin", "Casidhe", "Cassimir", "Cathan", "Cathaoirmor", "Cathasach", "Cathmaol", "Ceallach", "Ceannfhionn", "Ceara", "Cearbhallain", "Cearnach", "Cearrbhach", "Ceileachan", "Cein", "Cellanir", "Ceneric", "Ceran", "Chalice", "Chandiris", "Charea", "Cianan", "Ciarda", "Cillcumhan", "Cillin", "Cinfhaolaidh", "Cingesleah", "Cinnard", "Cinneididh", "Cinnfhail", "Ciulthinn", "Claefer", "Claeg", "Cleve", "Clif", "Clywd", "Coal", "Coalan", "Coed", "Coilin", "Coille", "Coinneach", "Coire", "Conaire", "Conan", "Conn", "Conndchadh", "Corbmac", "Corcurachan", "Corelja", "Corondal", "Corondhal", "Corzar", "Craccas", "Creag", "Creaga", "Creiddylad", "Creya", "Cristin", "Cuinn", "Curadhan", "Cuthbeorht", "Cwen", "Cwladys", "Cynbel", "Cyne", "Cyneburhleah", "Cyneric", "Cynesige", "Cyrius", "Cythranil", "Daegelbeorht", "Daegeseage", "Dael", "Daeltun", "Daeran", "Daghat", "Dagian", "Dagomar", "Dagr", "Daimhin", "Dalach", "Dalr", "Dalyell", "Danr", "Daregas", "Darhan", "Dariel", "Darwyn", "Dearan", "Deardrui", "Deasach", "Deasmumhan", "Debroun", "Defyaio", "Delair", "Dellingr", "Demandred", "Demyavan", "Dene", "Denethor", "Denu", "Deorward", "Dercarat", "Derenai", "Derylynn", "Dewi", "Dewi", "Diamar", "Diarmaoid", "Dikibyr", "Diolmhain", "Diomassach", "Direa", "Diss", "Doghailen", "Dogrim", "Doire", "Doireann", "Domhnull", "Dorminil", "Draca", "Drugiself", "Dryw", "Dseoran", "Du", "Duana", "Dubh", "Dubhgan", "Dubhghall", "Dubhglas", "Dubhlachan", "Dubhloach", "Dubhthach", "Duddaleah", "Dufrhealh", "Duhlasar", "Dumond", "Dunleah", "Dunn", "Dyddplentyn", "Dylan", "Dylan", "Eachan", "Eachthighearn", "Eada", "Eadbeorht", "Eadgar", "Eadmund", "Eadwulf", "Ealadhach", "Ealdraed", "Ealhard", "Ealhdun", "Eamon", "Eanruig", "Earnest", "Earric", "Eathelin", "Eatun", "Eberk", "Eburhardt", "Ecgbeorth", "Eferhard", "Efrania", "Ehren", "Eibhlin", "Eideann", "Eilis", "Einher", "Einion", "Eiric", "Eirik", "Eister", "Elanear", "Eldrias", "Elemthain", "Ellinar", "Elram", "Elrias", "Elspe", "Elsurion", "Endover", "Engelbergt", "Engholm", "Enit", "Eodoaine", "Eoghan", "Eoin", "Eorforwic", "Eorl", "Eostre", "Erinn", "Erminric", "Ertha", "Estcot", "Esthandir", "Esyathol", "Ethiyanil", "Eyrekr", "Eysellt", "Faegan", "Faeroth", "Faerrleah", "Faerven", "Faerwald", "Fairhinath", "Famek", "Faodhagan", "Fearbhirigh", "Fearghal", "Fearghus", "Fearn", "Feich", "Felabeorht", "Felizitas", "Fender", "Feoras", "Fiamar", "Filmaen", "Fingolfin", "Fionn", "Fionnghalac", "Fionnghuala", "Fips", "Firlionel", "Flanna", "Fleotig", "Floinn", "Flynt", "Fridu", "Friduric", "Frimunt", "Fugentun", "Gaelan", "Gaelbhan", "Galchobhar", "Gallgaidheal", "Gandalf", "Garisin", "Garivou", "Garm", "Garthr", "Garwig", "Geatan", "Genji", "Gerhwas", "Gerrod", "Gerwalt", "Ghleanna", "Gilolla", "Gimli", "Giollamhuire", "Giollaruaidh", "Gionnan", "Giorsal", "Gipcyan", "Gislbyr", "Gled", "Glenndun", "Glynydd", "Gnarf", "Gnimsch", "Gnosch", "Goathaire", "Goda", "Godehard", "Godgifu", "Gondo", "Goridh", "Goridh", "Gorman", "Gorman", "Goscelin", "Gothfraidh", "Grada", "Graegleah", "Griswald", "Gruffudd", "Gunnhar", "Guthr", "Gwalchmai", "Gwendolyn", "Gwenhwyvar", "Gwlsdys", "Gyldan", "Gyrwode", "Gytha", "Gyvron", "Hacor", "Hadu", "Haele", "Haesel", "Haestibgas", "Hafirinm", "Hafleikr", "Haga", "Hakon", "Halag", "Halfdan", "Halifrid", "Halig", "Haltor", "Hammar", "Hanraoi", "Haorinas", "Harad", "Haragraf", "Harailt", "Harpo", "Harti", "Haruald", "Hearpere", "Heathleah", "Heimrik", "Heort", "Heriberaht", "Herimann", "Herwig", "Hidlimar", "Hilbrand", "Hildhard", "Hohberht", "Hoibeard", "Hoireabard", "Holda", "Honod", "Howel", "Howel", "Hugiberaht", "Hugiet", "Hunfrid", "Hunig", "Iaian", "Ifig", "Iltak", "Imrahil", "Ingmar", "Iniadea", "Inis", "Iosep", "Isan", "Isedria", "Isenham", "Itu", "Ivhar", "Jami", "Jander", "Jaral", "Jeffries", "Jezer", "Joreg", "Jozan", "Kaja", "Kandorys", "Kerwyn", "Kiarr", "Kief", "Kiollsig", "Kirkja", "Kirkjabyr", "Knut", "Kort", "Korulas", "Krak", "Krossbyr", "Kuambyr", "Kulbari", "Kunagnos", "Kuonraed", "Kyan", "Kythauriel", "Labhruinn", "Ladhaoise", "Laec", "Lagan", "Laghras", "Laird", "Landbercht", "Langr", "Laochailan", "Laudrius", "Leagorn", "Leamhnach", "Leander", "Leannan", "Leathlaghra", "Lebennin", "Lefael", "Leif", "Leoma", "Leraneal", "Leschko", "Leskoh", "Lethanon", "Leutpald", "Lilias", "Lind", "Lindael", "Lindberg", "Lintflas", "Lioslaith", "Liusadh", "Llwyd", "Llyn", "Llyweilun", "Logmann", "Lokti", "Lomarin", "Lonn", "Lothar", "Lotharingen", "Lubig", "Lughaidh", "Lughaidh", "Luighseacg", "Luisadh", "Lundr", "Luthais", "Lyrandis", "Lyrsil", "Lysil", "Lysira", "Maarkan", "Mab", "Macothiel", "Madelhari", "Maegth", "Maeva", "Magafeld", "Magnus", "Maible", "Maighdlin", "Maire", "Mairghread", "Mairi", "Maithilis", "Mandel", "Mannfrith", "Maodighomhnaigh", "Maolmin", "Maolmin", "Maolmuire", "Maoltuile", "Marcail", "Maredud", "Mari", "Maril", "Marla", "Maskol", "Maura", "Maureen", "Meadhbh", "Mearr", "Meginhardt", "Meliondor", "Meredydd", "Merehloew", "Mersc", "Messkir", "Metira", "Metrios", "Mhari", "Mialee", "Micheil", "Minarvos", "Minata", "Mirtek", "Miureall", "Modread", "Mog-Macha", "Moibeal", "Moineruadh", "Moineruadh", "Moire", "Moireach", "Moldrack", "Monca", "Morag", "Morcan", "Morfinn", "Morgant", "Morgen", "Morogh", "Mortun", "Moya", "Muir", "Muire", "Muireadhaigh", "Muirgheal", "Muirne", "Murchadh", "Murthuile", "Mylnburne", "Naheniel", "Nathondal", "Naul", "Neblehle", "Nerviar", "Newyddllyn", "Niaeha", "Niall", "Nichus", "Niewheall", "Norberaht", "Nuallan", "Odbert", "Odharait", "Odhrean", "Odimorr", "Odwulf", "Oleifr", "Ollaneg", "Olvaerr", "Omid", "Oona", "Oonagh", "Ordalf", "Orharikr", "Osbeorht", "Oskar", "Osmaer", "Osraed", "Osric", "Othomann", "Owein", "Owein", "Padraig", "Padriac", "Paduicg", "Parlan", "Parlan", "Peadair", "Peadar", "Pennleah", "Peppi", "Perin", "Permeyah", "Preostleah", "Quarz", "Radagast", "Rafmag", "Allweg", "Ragdal", "El", "Zoreh", "Raghallach", "Raghnall", "Raginmund", "Rahn", "Raiola", "Raja", "Ramiris", "Randwulf", "Raoghnait", "Raskogr", "Rauthuellir", "Raymir", "Readwulf", "Regaf", "Regdar", "Reginberaht", "Reidhachadh", "Rhinfflew", "Rhuk", "Rhydag", "Rhys", "Riagan", "Rian", "Ridere", "Rikar", "Rille", "Riocard", "Riodhr", "Rioghbhardan", "Rioghnach", "Rodhlann", "Rognuald", "Rois", "Ronan", "Rotland", "Ruadhan", "Ruarc", "Rudrik", "Rudugeard", "Rumenea", "Ruodger", "Ruodlant", "Ruomhildr", "Rurik", "Sadhbh", "Sadhbha", "Saegar", "Saelec", "Saerfren", "Saeweard", "Saidhghin", "Sailbheastar", "Saitham", "Sala", "Salaidh", "Salasu", "San", "Rhaal", "Saphir", "Saretus", "Sargas", "Saxon", "Scanlan", "Sceaphierde", "Scelfleah", "Schiraljie", "Scirwode", "Scolaighe", "Scrileadh", "Seadaidh", "Seain", "Seanachan", "Seanan", "Seanlaoch", "Seann", "Secgleah", "Seiradan", "Selvagitas", "Sentaia", "Sgeulaiche", "Sha", "Rell", "Sha'Red", "Shane", "Shauir", "Sibeal", "Siddael", "Sigifrith", "Sigilwig", "Sigimund", "Sigiwald", "Signi", "Sigurdhr", "Silanay", "Silmalinnon", "Silmarilon", "Silviara", "Sim", "Sindira", "Sine", "Siobhan", "Siodhachan", "Siolta", "Siomonn", "Sion", "Sith`e`thak", "Siubhan", "Siudhne", "Siusan", "Skentha", "Skereye", "Skorag", "Skypr", "Slaedr", "Slaghan", "Sliaghin", "Solamh", "Somahirle", "Sorcha", "Sruthair", "Sruthan", "Stanach", "Steorra", "Stodhierde", "Strom", "Sucram", "Suileabhan", "Suthrland", "Swynedd", "Tabbert", "Tad", "Taffy", "Taithleach", "Tamnais", "Taran", "Taurelias", "Tearlach", "Teimhnean", "Temara", "Tendrik", "Tespius", "Tewdwr", "Thalion", "Thamios", "Tharimis", "Thegn", "Theuobald", "Theuroik", "Thoidgeirford", "Thoraths", "Thorbiartr", "Thorbiorn", "Thorfin", "Thorir", "Thoud", "Throaldr", "Thruhleow", "Thrythwig", "Ti'ak", "Tighearnach", "Tioboid", "Tiomoid", "Tirell", "Togtar", "Toirdealbach", "Toireasa", "Tomas", "Torc", "Tordek", "Torm", "Tormaigh", "Torr", "Torra", "Tosdramos", "Trahayarn", "Tramiel", "Trea", "Treabhar", "Treasach", "Trekarraz", "Trent", "Trevelian", "Trystan", "Tsoladin", "Tuathal", "Turgal", "Txorass", "Tygr", "Tyrion", "Ualtar", "Udo", "Uigboern", "Uilleam", "Uinsionn", "Ulbon", "Ulfmaerr", "Ulvelaik", "Unnurr", "Vaasa", "Valadenya", "Valerius", "Varin", "Varvia", "Vollmr", "Vychan", "Wace", "Waenwryht", "Waescburne", "Waldramm", "Walijan", "Wallihelm", "Wandi", "Wann", "Waren", "Warto", "Wendido", "Wenis", "Werro", "Wigis", "Willaperht", "Willimod", "Winiholdo", "Wolf", "Wudoreafa", "Wulfgar", "Wulfric", "Wulfrith", "Wyrduàn", "Yaligan", "Yarrik", "YaYarzar", "Yedda", "Yofenia", "Zaasz", "Zareius", "Zarrag", "Zolt", "Abvia", "Adalheit", "Aeldra", "Aelfdene", "Aeltra", "Aemete", "Aethelmaere", "Aidan", "Ailin", "Aimil", "Aine", "Airleas", "Aislinn", "Alain", "Alaria", "Allsun", "Alundra", "Alviss", "Amhiunn", "Andaria", "Aoiffe", "Astryd", "Athalindi", "Attheneldre", "Aylen", "Baduhildi", "Baldwine", "Banbrigge", "Beathag", "Bebhinn", "Beorhthildi", "Berahta", "Berangari", "Bloddwyn", "Brangwen", "Brann", "Breandan", "Bridhid", "Brita", "Bronwyn", "Brunihildi", "Cadhla", "Caellach", "Caitlin", "Caomhiun", "Ceara", "Chodhildi", "Ciarda", "Conn", "Creiddylad", "Cristin", "Cwladys", "Dalaria", "Damneya", "Deardrui", "Deorawine", "Doire", "Doireann", "Domhnull", "Duana", "Dyddplentyn", "Eadgyth", "Ealasaid", "Earwine", "Eibhlin", "Eideann", "Eilis", "Eister", "Elspe", "Engelberhta", "Enit", "Eodoaine", "Eorlariel", "Erinn", "Eysellt", "Fionnghuala", "Flanna", "Freyja", "Gala", "Gertrut", "Ghleanna", "Gilsberhta", "Giorsal", "Gisela", "Glynydd", "Grisjahildi", "Gunnhild", "Gwendolyn", "Gwenhwyvar", "Gwlsdys", "Haduwig", "Herthe", "Herwig", "Hilde", "Hildieth", "Hildigard", "Hlutwig", "Hrothwine", "HuldraIda", "Iduna", "ImmaIngrida", "Itu", "Kelda", "Ladhaoise", "Larissa", "Lidda", "Lilias", "Liusadh", "Luighseacg", "Luisadh", "Mab", "Maertisa", "Maeva", "Magamhildi", "Mahthildin", "Maible", "Maighdlin", "Maire", "Mairghread", "Mairi", "Marcail", "Maredud", "Mathildi", "Maura", "Maureen", "Meadhbh", "Mearr", "Mercia", "Meredydd", "Mhari", "Mildraed", "Minne", "Miureall", "Moibeal", "Moire", "Moireach", "Monca", "Morag", "Morgant", "Moya", "Muire", "Muirgheal", "Muirne", "Nadjala", "Niall", "Odharait", "Oona", "Oonagh", "Ordwime", "Pianwig", "Raginmund", "Raoghnait", "Rioghnach", "Rois", "Rozumund", "Ruomhildr", "Sadhbh", "Sadhbha", "Saidhghin", "Salaidh", "Sibeal", "Sigilwig", "Sigimund", "Signi", "Sine", "Siobhan", "Sion", "Siubhan", "Siusan", "Sorcha", "Sosanna", "Swynedd", "Taithleach", "Tanya", "Thoridyss", "Toirdealbach", "Toireasa", "Torma", "Torr", "Torra", "Truda", "Ula", "Ura", "Walda", "Waldburga", "Winifrid", "Wulfila", "Wulfrith", "Wulfsige" };
    public static string GetRandomName()
    {
        int i = WorldMain.Random.RandiRange(0, NameList.Count - 1);
        return NameList[i];
    }

    public Weapon Weapon;
    public Timer ActionTimer;
    public Timer ActionCooldown;
    bool action = false;
    bool cooldown = false;

    Area2D SearchArea;

    public static Player GetNextPlayer()
    {
        Array<Node> nodes = WorldMain.Instance.Map.GetChildren();

        Player first = null;
        bool foundCurrent = false;
        foreach (Node n in nodes)
        {
            if (n is Player player)
            {
                if (first == null)
                    first = player;

                if (foundCurrent && player != WorldMain.SelectedObject)
                    return player;

                if (player == WorldMain.SelectedObject)
                    foundCurrent = true;
            }
        }

        if(first != null)
            return first;

        return null;
    }

    public override void _Ready()
    {
        base._Ready();
        Weapon = GetNode<Weapon>("Weapon");

        ActionTimer = GetNode<Timer>("ActionTimer");
        ActionTimer.Timeout += OnTimerTimeout;

        ActionCooldown = GetNode<Timer>("ActionCooldown");
        ActionCooldown.Timeout += OnTimerCoolDownTimeout;

        SearchArea = GetNode<Area2D>("Area2DSearch");

        Area2D area = GetNode<Area2D>("Area2D");
        area.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if(Target is Node2D)
        {
            if(body == Target)
            {
                if(body.IsInGroup("Breakable") || (body.IsInGroup("Animal") && body.IsInGroup("R_Food")))
                    State = GameObjectState.FIGHTING;

                if (body.IsInGroup("Storable"))
                {
                    Target = null;
                    State = GameObjectState.IDLE;
                }
            }
        }
    }

    public void OnTimerTimeout()
    {
        Weapon.Hide();
        Weapon.CollisionShape.Disabled = true;
        action = false;
        cooldown = true;
        ActionCooldown.Start();
    }

    public void OnTimerCoolDownTimeout()
    {
        cooldown = false;
    }

    public new void OnInputEvent(Node Viewport, InputEvent @event, long shapeIdx)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            GD.Print("Player CLICK");
            WorldMain.SelectedObject = this;
        }
    }


    public override void _Process(double delta)
    {
        PlayerWeapon();        
        Movement();

        if (!action)
        {
            UpdateAnimation();
        }

        if(State == GameObjectState.WAITING)
        {
            State = GameObjectState.IDLE;
            return;
        }


        if(State == GameObjectState.IDLE)
        {
            SetNextState();
        }
    }

    public void SetSearch(string search)
    {
        if(search != null)
        {
            Node2D target = SearchNextResource(search);
            SetTarget(target);
        }
        else
            State = GameObjectState.IDLE;
    }


    void SetNextState()
    {
        if(Mission != null)
        {
            if(Mission.State == GameObjectState.FARMING)
            {
                string search = (string)Mission.Target;

                Node2D target;
                if(Inventory.CountItemGroup(search) == 0)
                    target = SearchNextResource(search);
                else
                {
                    target = SearchNextResource("Storable");
                }

                State = GameObjectState.WALKING;
                Target = target;
            }
        }
    }

    Node2D SearchNextResource(string groupName)
    {
        var objs = SearchArea.GetOverlappingBodies();
        objs.AddRange(SearchArea.GetOverlappingAreas());

        Array<Node2D> nodes = new Array<Node2D>();

        foreach (Node2D obj in objs)
        {
            if (obj.IsInGroup(groupName))
            {
                nodes.Add(obj);
            }
        }

        Node2D nearest = null;
        float min = float.MaxValue;
        foreach (Node2D obj in nodes)
        {
            float dist = Position.DistanceTo(obj.Position);
            if(dist < min)
            {
                min = dist;
                nearest = obj;
            }
        }

        return nearest;
    }


    public void PlayerWeapon()
    {
        if(!action && !cooldown 
            && ((WorldMain.SelectedPlayer == this && Input.IsActionJustPressed("attack"))
                    || State == GameObjectState.FIGHTING))
        {
            Weapon.Show();
            Animator.Play("attack_" + Direction);
            AnimatorShadow.Play("attack_" + Direction);
            Weapon.CollisionShape.Disabled = false;
            ActionTimer.Start();
            action = true;
        }
    }


    public void Collect(InventoryItem item, int amount = 1)
    {
        Inventory.Insert(item, amount);
    }

    internal void SetTarget(Vector2 vector2)
    {
        State = GameObjectState.WALKING;
        Target = vector2;
    }

    internal void SetTarget(Node2D node)
    {
        Target = node;
        State = node == null ? GameObjectState.IDLE : GameObjectState.WALKING;

        if(node == null && !Inventory.IsEmpty)
        {
            Node2D nearest = null;
            float min = float.MaxValue;
            var storages = GetTree().GetNodesInGroup("Storable");
            foreach (Node2D store in storages)
            {
                float dist = Position.DistanceTo(store.Position);
                if(dist < min)
                {
                    min = dist;
                    nearest = store;
                }
            }

            SetTarget(nearest);
        }
    }

    

    public override void Movement()
    {
        Velocity = Vector2.Zero;

        //TODO: Only in first Person
        if (WorldMain.SelectedPlayer == this)
            Velocity = Input.GetVector("left", "rigth", "up", "down");

        if (State == GameObjectState.WALKING && Target != null)
        {
            Vector2 targetPos =  Target is Vector2 ? (Vector2)Target : ((Node2D)Target).Position;

            Vector2 direction = (targetPos - GlobalPosition).Normalized();
            Velocity = direction;
        }

        Velocity *= Speed;

        MoveAndSlide();
    }

    public override void UpdateAnimation()
    {
        if (Velocity.X > 0) Direction = "r";
        else if (Velocity.X < 0) Direction = "l";
        else if (Velocity.Y > 0) Direction = "d";
        else if (Velocity.Y < 0) Direction = "u";

        string animationName;

        if(Velocity == Vector2.Zero)
            animationName = "idle_" + Direction;
        else
            animationName= "walk_" + Direction;

        Animator.Play(animationName);
        AnimatorShadow.Play(animationName);
    }
}
