using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Threading;
using KModkit;
using System.Linq;


public class BooleanWiresScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;
    public KMBombModule Module;

    public string firstentered = "";
    public string secondentered = "";
    public string thirdentered = "";
    public string fourthentered = "";
    public string fifthentered = "";
    public KMSelectable[] Wires;
    public KMSelectable TrueWire;
    public KMSelectable FalseWire;
    public GameObject CutWire;
    public GameObject Text1;
    public GameObject Text2;
    public bool IsFirstFalse = false;
    public bool IsSecondFalse = false;
    public List<int> notCuts;
    public List<string> letters = new List<string>();
    public Mesh mesh;
    public Mesh cutMesh;
    public Mesh noncutMesh;
    public int stageNumber = 0;
    public string firstLetter = "";
    public string secondLetter = "";
    public string input = "";
    public string correctAnswer = "";
    
    public List<string> Corrects;
    public List<string> Entered;
    public float OriginalTime;
    public bool isOrUsed = false;
    public bool isNandUsed = false;

    private static int moduleIdCounter;
    private int moduleId;
    private bool solved;

    public GameObject WiresParentObj;
    public GameObject DoorObj;
    private bool _canInteract = true;

    private void Start()
    {
        OriginalTime = Bomb.GetTime();
    }

    private void Awake()
    {
        moduleId = moduleIdCounter++;

        for (int i = 0; i <= 8; i++)
            notCuts.Add(i);

        mesh = CutWire.GetComponent<MeshFilter>().sharedMesh;
        cutMesh = Instantiate(mesh);
        mesh = Wires[0].GetComponent<MeshFilter>().sharedMesh;
        noncutMesh = Instantiate(mesh);

        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < 26; i++)
            letters.Add(alphabet[i].ToString());

        for (int i = 0; i <= 8; i++)
        {
            int j = i;
            Wires[i].OnInteract += delegate ()
            {
                if (_canInteract)
                    snipWire(j);
                return false;
            };
        }

        FalseWire.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (!_canInteract)
                return false;
            FalseWire.AddInteractionPunch(.5f);
            StartCoroutine(RefreshPanel(false));
            return false;
        };

        TrueWire.OnInteract += delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (!_canInteract)
                return false;
            if (!solved)
            {
                TrueWire.AddInteractionPunch(.5f);
                input = Numerate();
                Debug.LogFormat("[Boolean Wires #{0}] Entered operator: {1}", moduleId, input);
                Entered.Add(input);
                Calculate();
                if (Corrects.Contains(input))
                {
                    Debug.LogFormat("[Boolean Wires #{0}] Correct!", moduleId);
                    if (input == "OR") { isOrUsed = true; }
                    if (input == "NAND") { isNandUsed = true; }
                    Entered.Add(input);
                    StartCoroutine(RefreshPanel(true));
                }
                else
                {
                    Debug.LogFormat("[Boolean Wires #{0}] Incorrect!", moduleId);
                    stageNumber = 0;
                    Entered.Clear();
                    isOrUsed = false;
                    isNandUsed = false;
                    StartCoroutine(RefreshPanel(true));
                    Module.HandleStrike();
                }
            }
            return false;
        };
        StartCoroutine(RefreshPanel(true));

    }
    private void Calculate()
    {
        bool first = getstate(firstLetter, 1);
        bool second = getstate(secondLetter, 2);
        //Debug.LogFormat("States are: {0} {1}", first, second);
        if (first ^ second) { Corrects.Add("XOR"); }
        if (first && second) { Corrects.Add("AND"); }
        if (first || second) { if (!isOrUsed) { Corrects.Add("OR"); } }
        if (!first && !second) { Corrects.Add("NOR"); }
        if (!(first && second)) { if (!isNandUsed) { Corrects.Add("NAND"); } }
    }

    private void generateNew()
    {
        Corrects.Clear();
        stageNumber++;
        Reset();

        if (stageNumber < 6)
        {
            firstLetter = letters[UnityEngine.Random.Range(0, 26)];
            Text1.GetComponent<TextMesh>().text = firstLetter;
            secondLetter = letters[UnityEngine.Random.Range(0, 26)];
            Text2.GetComponent<TextMesh>().text = secondLetter;
            while (firstLetter == secondLetter)
            {
                secondLetter = letters[UnityEngine.Random.Range(0, 26)];
                Text2.GetComponent<TextMesh>().text = secondLetter;
            }
            int rdm = UnityEngine.Random.Range(0, 2);
            if (rdm == 1)
            {
                IsFirstFalse = true;
                Text1.GetComponent<TextMesh>().text = "!" + Text1.GetComponent<TextMesh>().text;
            }
            rdm = UnityEngine.Random.Range(0, 2);
            if (rdm == 1)
            {
                IsSecondFalse = true;
                Text2.GetComponent<TextMesh>().text = "!" + Text2.GetComponent<TextMesh>().text;
            }
        }
        else
        {
            solved = true;
            Text1.GetComponent<TextMesh>().text = "";
            Text2.GetComponent<TextMesh>().text = "";
            firstentered = Entered[0];
            secondentered = Entered[1];
            thirdentered = Entered[2];
            fourthentered = Entered[3];
            fifthentered = Entered[4];
            Debug.LogFormat("[Boolean Wires #{0}] Module solved!", moduleId);
            Module.HandlePass();
        }
    }


    private string Numerate()
    {
        if (notCuts.Count == 2 && notCuts.Contains(6) && notCuts.Contains(7))
            return "AND";
        else
            if (notCuts.Count == 2 && notCuts.Contains(4) && notCuts.Contains(5))
                return "OR";
            else
                if (notCuts.Count == 3 && notCuts.Contains(4) && notCuts.Contains(5) && notCuts.Contains(2))
                    return "XOR";
                else
                    if (notCuts.Count == 3 && notCuts.Contains(8) && notCuts.Contains(4) && notCuts.Contains(5))
                        return "NOR";
                    else
                        if ((notCuts.Count == 1 && notCuts.Contains(1)) || (notCuts.Count == 1 && notCuts.Contains(3)) || (notCuts.Count == 1 && notCuts.Contains(8)))
                            return "NAND";
                        else
                            return "Invalid";
    }

    private bool getstate(string letter, int num)
    {
        bool temp = false;
        //Debug.LogFormat("[Boolean Wires ] Isfalse:  {0}", isfalse);
        switch (letter)
        {
            case "A":
                if (Bomb.GetSerialNumberLetters().Any("AEIOU".Contains)) temp = true;
                break;
            case "B":
                if (Bomb.GetBatteryCount(Battery.D) > 1) temp = true;
                break;
            case "C":
                if ((Bomb.GetSerialNumberNumbers().Last() % 2) == 0) temp = true;
                break;
            case "D":
                if ((Bomb.GetModuleNames().Count % 2) == 0) temp = true;
                break;
            case "E":
                if (Bomb.GetModuleNames().Contains("Forget Me Not")) temp = true;
                break;
            case "F":
                if (Bomb.IsIndicatorOn(Indicator.BOB)) temp = true;
                break;
            case "G":
                if (Bomb.IsIndicatorOff(Indicator.CAR)) temp = true;
                break;
            case "H":
                if (Bomb.GetBatteryCount() == 0) temp = true;
                break;
            case "I":
                if (Bomb.GetBatteryCount(Battery.AA) == 2) temp = true;
                break;
            case "J":
                if (isPrime(Bomb.GetModuleNames().Count)) temp = true;
                break;
            case "K":
                if (Bomb.GetTime() < (OriginalTime / 2)) temp = true;
                break;
            case "L":
                if (Bomb.IsPortPresent(Port.Parallel)) temp = true;
                break;
            case "M":
                if (Bomb.IsPortPresent(Port.Serial)) temp = true;
                break;
            case "N":
                bool doublePort = false;
                HashSet<string> portList = new HashSet<string>();
                foreach (string s in KMBombInfoExtensions.GetPorts(Bomb))
                {
                    if (portList.Contains(s))
                    {
                        doublePort = true;
                        break;
                    }
                    portList.Add(s);
                }
                if (doublePort) temp = true;
                break;
            case "O":
                if (Bomb.IsIndicatorOff(Indicator.MSA)) temp = true;
                break;
            case "P":
                if (Bomb.IsIndicatorOn(Indicator.FRQ)) temp = true;
                break;
            case "Q":
                temp = true;
                break;
            case "R":
                if ((Bomb.GetModuleNames().Count) % 3 == 0) temp = true;
                break;
            case "S":
                if ((Bomb.GetModuleNames().Count) > (OriginalTime / 60)) temp = true;
                break;
            case "T":
                if ((Bomb.GetModuleNames().Count) > 71) temp = true;
                break;
            case "U":
                if ((Bomb.GetSolvedModuleNames().Count()) > ((Bomb.GetSolvableModuleNames().Count()) - (Bomb.GetSolvedModuleNames().Count()))) temp = true;
                break;
            case "V":
                if ((Bomb.GetSolvedModuleNames().Count()) % 2 == 0) temp = true;
                break;
            case "W":
                if (Bomb.IsPortPresent(Port.PS2)) temp = true;
                break;
            case "X":
                if (Bomb.IsPortPresent(Port.StereoRCA)) temp = true;
                break;
            case "Y":
                if (Bomb.GetOnIndicators().Count() == Bomb.GetOffIndicators().Count()) temp = true;
                break;
            case "Z":
                if (Bomb.GetOnIndicators().Count() == 0 && Bomb.GetOffIndicators().Count() == 0) temp = true;
                break;
        }
        if (num == 1)
        {
            if ((Text1.GetComponent<TextMesh>().text.Contains("!")))
                return !temp;
            else
                return temp;
        }
        else
        {
            if ((Text2.GetComponent<TextMesh>().text.Contains("!")))
                return !temp;
            else
                return temp;
        }
    }

    private bool isPrime(int number)
    {
        if (number == 1)
            return false;
        else
        {
            int n, i, m = 0, flag = 0;
            n = number;
            m = n / 2;
            for (i = 2; i <= m; i++)
                if (n % i == 0)
                    return false;
            return flag == 0;
        }
    }

    private void snipWire(int wirenum)
    {
        if (notCuts.Contains(wirenum))
        {
            notCuts.Remove(wirenum);
            Wires[wirenum].GetComponent<MeshFilter>().sharedMesh = cutMesh;
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, transform);
        }
    }

    private IEnumerator RefreshPanel(bool regen)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSequenceMechanism, transform);
        _canInteract = false;
        var duration = 0.2f;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            WiresParentObj.transform.localPosition = new Vector3(-0.0049f, Mathf.Lerp(0f, -0.013f, elapsed / duration), -0.014f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        WiresParentObj.transform.localPosition = new Vector3(-0.0049f, -0.013f, -0.014f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            DoorObj.transform.localPosition = new Vector3(-0.01714f, 0.0114f, Mathf.Lerp(0.03672f, -0.01328f, elapsed / duration));
            DoorObj.transform.localScale = new Vector3(0.11f, Mathf.Lerp(0f, 0.1f, elapsed / duration), 1f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        DoorObj.transform.localPosition = new Vector3(-0.01714f, 0.0114f, -0.01328f);
        DoorObj.transform.localScale = new Vector3(0.11f, 0.1f, 1f);
        if (regen)
            generateNew();
        Reset();
        elapsed = 0f;
        while (elapsed < duration)
        {
            DoorObj.transform.localPosition = new Vector3(-0.01714f, 0.0114f, Mathf.Lerp(-0.01328f, 0.03672f, elapsed / duration));
            DoorObj.transform.localScale = new Vector3(0.11f, Mathf.Lerp(0.1f, 0f, elapsed / duration), 1f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        DoorObj.transform.localPosition = new Vector3(-0.01714f, 0.0114f, 0.03672f);
        DoorObj.transform.localScale = new Vector3(0.11f, 0f, 1f);
        elapsed = 0f;
        while (elapsed < duration)
        {
            WiresParentObj.transform.localPosition = new Vector3(-0.0049f, Mathf.Lerp(-0.013f, 0f, elapsed / duration), -0.014f);
            yield return null;
            elapsed += Time.deltaTime;
        }
        WiresParentObj.transform.localPosition = new Vector3(-0.0049f, 0f, -0.014f);
        _canInteract = true;
        yield break;
    }

    private void Reset()
    {
        notCuts.Clear();
        for (int i = 0; i <= 8; i++)
        {
            Wires[i].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
            notCuts.Add(i);
        }
    }

    private static readonly int[][] _logicOperatorsForTP = new int[][]
    {
        new int[] { 0, 1, 2, 3, 6, 7, 8 },
        new int[] { 0, 1, 2, 3, 4, 5, 8 },
        new int[] { 0, 1, 3, 6, 7, 8 },
        new int[] { 0, 1, 2, 3, 6, 7 },
        new int[] { 0, 1, 2, 4, 5, 6, 7, 8 }
    };

    public string TwitchHelpMessage = "Use '!{0} submit <operator>' to submit the specified operator! Valid operators are: OR; XOR; AND; NAND; NOR.";
    IEnumerator ProcessTwitchCommand(string command)
    {
        int[] wires;
        if (command.Equals("submit or", StringComparison.InvariantCultureIgnoreCase))
            wires = _logicOperatorsForTP[0];
        else if (command.Equals("submit and", StringComparison.InvariantCultureIgnoreCase))
            wires = _logicOperatorsForTP[1];
        else if (command.Equals("submit xor", StringComparison.InvariantCultureIgnoreCase))
            wires = _logicOperatorsForTP[2];
        else if (command.Equals("submit nor", StringComparison.InvariantCultureIgnoreCase))
            wires = _logicOperatorsForTP[3];
        else if (command.Equals("submit nand", StringComparison.InvariantCultureIgnoreCase))
            wires = _logicOperatorsForTP[4];
        else
            yield break;
        yield return null;
        while (!_canInteract)
            yield return null;
        for (int i = 0; i < wires.Length; i++)
        {
            Wires[wires[i]].OnInteract();
            yield return new WaitForSeconds(0.05f);
        }
        TrueWire.OnInteract();
    }
}
