using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mos6510;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class TestMos : MonoBehaviour {

	private ProgrammingModel model;
	private Memory memory;
	private Repl repl;

	private int instructionCounter;

	public InputField assemblyInstructionInputField;
	public Text registerOutputText;
	public Text instructionsText;
	public Text errorText;
	public Button executeButton;

	// Use this for initialization
	void Start ()
	{
		model = new ProgrammingModel();
		memory = new Memory();
		repl = new Repl(model, memory);
		instructionCounter = 1;
		registerOutputText.text = repl.PrintRegisters().TrimStart();
		errorText.enabled = false;
	}

	void Update ()
	{
		if (string.IsNullOrEmpty(assemblyInstructionInputField.text))
			executeButton.interactable = false;
		else
			executeButton.interactable = true;
	}

	public void OnExecute()
	{
		var assemblyCode = assemblyInstructionInputField.text;
		if (repl.TryRead(assemblyCode))
		{
			RecordInstructionViaAnalytics(assemblyCode);
            instructionsText.text += string.Format("\n {0}: {1}", instructionCounter, assemblyCode);
			instructionCounter++;
			repl.Execute();
			registerOutputText.text = repl.PrintRegisters().TrimStart();
			errorText.enabled = false;
		}
		else
		{
			errorText.text = string.Format("Unknown assembly code: '{0}'", assemblyCode);
			errorText.enabled = true;
		}
	}

	public void OnEndEdit()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			OnExecute();
		}
	}

	private void RecordInstructionViaAnalytics(string assemblyCode)
	{
		Analytics.CustomEvent("InstructionExecuted", new Dictionary<string, object>
		{
			{ "assemblyCode", assemblyCode },
		});
	}


}
