using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using System.Threading;
using KModkit;
using System.Linq;


public class BooleanWiresScript : MonoBehaviour {
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
	public List<string> letters;
	public Mesh mesh;
	public Mesh cutMesh;
	public Mesh noncutMesh;
	public KMAudio Audio;
	public int stageNumber = 0;
	public string firstLetter = "";
	public string secondLetter = "";
	public string input = "";
	public string correctAnswer = "";
	public bool solved = false;
	public List<string> Corrects;
	public KMBombInfo Bomb;
	public List<string> Entered;
	public float OriginalTime;
	public bool isOrUsed = false;
	public bool isNandUsed = false;



	void Awake() {
		OriginalTime = Bomb.GetTime();
        letters.Add("A");
        letters.Add("B");
        letters.Add("C");
        letters.Add("D");
        letters.Add("E");
        letters.Add("F");
		letters.Add("G");
		letters.Add("H");
		letters.Add("I");
		letters.Add("J");
		letters.Add("K");
		letters.Add("L");
		letters.Add("M");
		letters.Add("N");
		letters.Add("O");
		letters.Add("P");
		letters.Add("Q");
		letters.Add("R");
		letters.Add("S");
		letters.Add("T");
		letters.Add("U");
		letters.Add("V");
		letters.Add("W");
		letters.Add("X");
		letters.Add("Y");
		letters.Add("Z");

		notCuts.Add(0);
		notCuts.Add(1);
		notCuts.Add(2);
		notCuts.Add(3);
		notCuts.Add(4);
		notCuts.Add(5);
		notCuts.Add(6);
		notCuts.Add(7);
		
		mesh = CutWire.GetComponent<MeshFilter>().sharedMesh;
        cutMesh = Instantiate(mesh);
		mesh = Wires[0].GetComponent<MeshFilter>().sharedMesh;
		noncutMesh = Instantiate(mesh);
        
		/*for(int i = 0; i<8; i++){
			Wires[i].OnInteract += delegate(){
				snipWire(i);
				return false;
			}
		;}*/
		Wires[0].OnInteract += delegate(){
				Wires[0].AddInteractionPunch(.5f);
				snipWire(0);
				return false;
			};
		Wires[1].OnInteract += delegate(){
				Wires[1].AddInteractionPunch(.5f);
				snipWire(1);
				return false;
			};
		Wires[2].OnInteract += delegate(){
				Wires[2].AddInteractionPunch(.5f);
				snipWire(2);
				return false;
			};
		Wires[3].OnInteract += delegate(){
				Wires[3].AddInteractionPunch(.5f);
				snipWire(3);
				return false;
			};
		Wires[4].OnInteract += delegate(){
				Wires[4].AddInteractionPunch(.5f);
				snipWire(4);
				return false;
			};
		Wires[5].OnInteract += delegate(){
				Wires[5].AddInteractionPunch(.5f);
				snipWire(5);
				return false;
			};
		Wires[6].OnInteract += delegate(){
				Wires[6].AddInteractionPunch(.5f);
				snipWire(6);
				return false;
			};
		Wires[7].OnInteract += delegate(){
				Wires[7].AddInteractionPunch(.5f);
				snipWire(7);
				return false;
			};

		FalseWire.OnInteract += delegate(){
			GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, transform);
			FalseWire.AddInteractionPunch(.5f);
			Wires[0].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[1].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[2].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[3].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[4].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[5].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[6].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			Wires[7].GetComponent<MeshFilter>().sharedMesh = noncutMesh;
			notCuts.Clear();
			notCuts.Add(0);
			notCuts.Add(1);
			notCuts.Add(2);
			notCuts.Add(3);
			notCuts.Add(4);
			notCuts.Add(5);
			notCuts.Add(6);
			notCuts.Add(7);
			return false;
		};

		TrueWire.OnInteract += delegate(){
			if(!solved){
			TrueWire.AddInteractionPunch(.5f);
			input = Numerate();
			Entered.Add(input);
			//Debug.LogFormat("[Boolean Wires #{0}] Got operator: " + input, 1);
			Calculate();
			if(Corrects.Contains(input)){
				if(input=="OR"){isOrUsed=true;}
				if(input=="NAND"){isNandUsed=true;}
				Entered.Add(input);
				generateNew();
			}
			else{
				stageNumber=0;
				Entered.Clear();
				isOrUsed = false;
				isNandUsed = false;
				generateNew();
				GetComponent<KMBombModule>().HandleStrike();
				}
			}
			return false;
		};
		
		generateNew();
		
	}
	void Calculate(){
		bool first = getstate(firstLetter, 1);
		bool second = getstate(secondLetter, 2);
		//Debug.LogFormat("States are: {0} {1}", first, second);
		if(first ^ second){Corrects.Add("XOR");}
		if(first && second){Corrects.Add("AND");}
		if(first || second){if(!isOrUsed){Corrects.Add("OR");}}
		if(!first && !second){Corrects.Add("NOT");}
		if(!(first && second)){if(!isNandUsed){Corrects.Add("NAND");}}
	}

	void generateNew(){
		Corrects.Clear();
		stageNumber++;
		FalseWire.OnInteract();

		if(stageNumber<6){

		firstLetter = letters[UnityEngine.Random.Range(0,26)];
		Text1.GetComponent<TextMesh>().text=firstLetter;
		secondLetter = letters[UnityEngine.Random.Range(0,26)];
		Text2.GetComponent<TextMesh>().text=secondLetter;
		while(firstLetter == secondLetter){
			secondLetter = letters[UnityEngine.Random.Range(0,26)];
			Text2.GetComponent<TextMesh>().text=secondLetter;
		}
		int rdm = UnityEngine.Random.Range(0,2);
		if(rdm==1){
			IsFirstFalse=true;
			Text1.GetComponent<TextMesh>().text="!" + Text1.GetComponent<TextMesh>().text;
		}
		rdm = UnityEngine.Random.Range(0,2);
		if(rdm==1){
			IsSecondFalse=true;
			Text2.GetComponent<TextMesh>().text="!" + Text2.GetComponent<TextMesh>().text;
		}
		}
		else{
			solved=true;
			Text1.GetComponent<TextMesh>().text="";
			Text2.GetComponent<TextMesh>().text="";
			firstentered=Entered[0];
			secondentered=Entered[1];
			thirdentered=Entered[2];
			fourthentered=Entered[3];
			fifthentered=Entered[4];
			GetComponent<KMBombModule>().HandlePass();
		}
	}


	public string Numerate(){
		if(notCuts.Count==2 && notCuts.Contains(6) && notCuts.Contains(7)){
			return "AND";
		}
		else{
			if(notCuts.Count==2 && notCuts.Contains(4) && notCuts.Contains(5)){
				return "OR";
			}
			else{
				if(notCuts.Count==3 && notCuts.Contains(4) && notCuts.Contains(5) && notCuts.Contains(2)){
					return "XOR";
				}
				else{
					if(notCuts.Count==2 && notCuts.Contains(0) && notCuts.Contains(1)){
						return "NOT";
					}
					else{
						if((notCuts.Count==1 && notCuts.Contains(1)) || (notCuts.Count==1 && notCuts.Contains(3))){
						return "NAND";
					}
						else{
						return "Invalid";
						}
					}
				}
			}
		}
	}

	public bool getstate(string letter, int num){
		bool temp = false;
		//Debug.LogFormat("[Boolean Wires ] Isfalse:  {0}", isfalse);
		switch(letter){
			case "A":
				if(Bomb.GetSerialNumberLetters().Any("AEIOU".Contains)){temp = true;}
				break;
			case "B":
				if(Bomb.GetBatteryCount(Battery.D) > 1){temp = true;}
				break;
			case "C":
				if((Bomb.GetSerialNumberNumbers().Last() % 2) == 0){temp = true;}
				break;
			case "D":
				if((Bomb.GetModuleNames().Count % 2) == 0){temp=true;}
				break;
			case "E":
				if(Bomb.GetModuleNames().Contains("Forget Me Not")){temp=true;}
				break;
			case "F":
				if (Bomb.IsIndicatorOn(Indicator.BOB)){temp=true;}
				break;
			case "G":
				if (Bomb.IsIndicatorOff(Indicator.CAR)){temp=true;}
				break;
			case "H":
				if(Bomb.GetBatteryCount() == 0){temp = true;}
				break;
			case "I":
				if(Bomb.GetBatteryCount(Battery.AA) == 2){temp = true;}
				break;
			case "J":
				if(isPrime(Bomb.GetModuleNames().Count)){temp=true;}
				break;
			case "K":
				if(Bomb.GetTime() < (OriginalTime/2)){temp=true;}
				break;
			case "L":
				if(Bomb.IsPortPresent(Port.Parallel)){temp=true;}
				break;
			case "M":
				if(Bomb.IsPortPresent(Port.Serial)){temp=true;}
				break;
			case "N":
				bool doublePort=false;
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
				if(doublePort){temp=true;}
				break;
			case "O":
				if (Bomb.IsIndicatorOff(Indicator.MSA)){temp=true;}
				break;
			case "P":
				if (Bomb.IsIndicatorOn(Indicator.FRQ)){temp=true;}
				break;
			case "Q":
				temp=true;
				break;
			case "R":
				if((Bomb.GetModuleNames().Count) %3 == 0){temp=true;}
				break;
			case "S":
				if((Bomb.GetModuleNames().Count) > ((Bomb.GetTime()) / 60)){temp=true;}
				break;
			case "T":
				if((Bomb.GetModuleNames().Count) >71){temp=true;}
				break;
			case "U":
				if((Bomb.GetSolvedModuleNames().Count()) > ((Bomb.GetSolvableModuleNames().Count()) - (Bomb.GetSolvedModuleNames().Count()))){temp=true;}
				break;
			case "V":
				if((Bomb.GetSolvedModuleNames().Count()) %2 ==0){temp=true;}
				break;
			case "W":
				if(Bomb.IsPortPresent(Port.PS2)){temp=true;}
				break;
			case "X":
				if(Bomb.IsPortPresent(Port.StereoRCA)){temp=true;}
				break;
			case "Y":
				if (Bomb.GetOnIndicators().Count() == Bomb.GetOffIndicators().Count()){temp=true;}
				break;
			case "Z":
				if (Bomb.GetOnIndicators().Count() == 0 && Bomb.GetOffIndicators().Count() == 0){temp=true;}
				break;
			}
		//Debug.LogFormat("[Boolean Wires ] Got {0}", temp);
		if(num==1){
		if((Text1.GetComponent<TextMesh>().text.Contains("!"))){
			//Debug.LogFormat("[Boolean Wires ] Returned {0}", !temp);
			return !temp;
		}
		else{
			//Debug.LogFormat("[Boolean Wires ] Returned {0}", temp);
			return temp;
		}}
		else{
			if((Text2.GetComponent<TextMesh>().text.Contains("!"))){
			//Debug.LogFormat("[Boolean Wires ] Returned {0}", !temp);
			return !temp;
			}
			else{
			//Debug.LogFormat("[Boolean Wires ] Returned {0}", temp);
			return temp;
			}
		}
		
		
	}

	public bool isPrime(int number){
		if(number==1){return false;}
		else{
		int n, i, m=0, flag=0;    
          n = number;  
          m=n/2;    
          for(i = 2; i <= m; i++)    
          {    
           if(n % i == 0)    
            {    
             return false;  
             flag=1;    
             break;    
            }    
          }    
          if (flag==0){    
           return true;}
		  else{
			  return false;
		  }
		}
	}

	void snipWire(int wirenum){
		//Debug.LogFormat("[Boolean Wires #{0}] Got to wire: " + wirenum+1, 1);
		if(notCuts.Contains(wirenum)){
			notCuts.Remove(wirenum);
			Wires[wirenum].GetComponent<MeshFilter>().sharedMesh = cutMesh;
			GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, transform);
		}
	}

	public string TwitchHelpMessage = "Use '!{0} submit <operator>' to submit the specified operator! Valid operators are: OR; XOR; AND; NAND; NOT.";
    IEnumerator ProcessTwitchCommand(string command)
    {
		yield return null;
		if(command.Equals("submit or", StringComparison.InvariantCultureIgnoreCase)){
			Wires[0].OnInteract();
			Wires[1].OnInteract();
			Wires[2].OnInteract();
			Wires[3].OnInteract();
			Wires[6].OnInteract();
			Wires[7].OnInteract();
			TrueWire.OnInteract();
		}
		if(command.Equals("submit and", StringComparison.InvariantCultureIgnoreCase)){
			Wires[0].OnInteract();
			Wires[1].OnInteract();
			Wires[2].OnInteract();
			Wires[3].OnInteract();
			Wires[4].OnInteract();
			Wires[5].OnInteract();
			TrueWire.OnInteract();
		}
		if(command.Equals("submit xor", StringComparison.InvariantCultureIgnoreCase)){
			Wires[0].OnInteract();
			Wires[1].OnInteract();
			Wires[3].OnInteract();
			Wires[6].OnInteract();
			Wires[7].OnInteract();
			TrueWire.OnInteract();
		}
		if(command.Equals("submit not", StringComparison.InvariantCultureIgnoreCase)){
			Wires[2].OnInteract();
			Wires[3].OnInteract();
			Wires[4].OnInteract();
			Wires[5].OnInteract();
			Wires[6].OnInteract();
			Wires[7].OnInteract();
			TrueWire.OnInteract();
		}
		if(command.Equals("submit nand", StringComparison.InvariantCultureIgnoreCase)){
			Wires[0].OnInteract();
			Wires[1].OnInteract();
			Wires[2].OnInteract();
			Wires[4].OnInteract();
			Wires[5].OnInteract();
			Wires[6].OnInteract();
			Wires[7].OnInteract();
			TrueWire.OnInteract();
		}
	}
}
