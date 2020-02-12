﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class BattleBox : TextAutotyper {

    [SerializeField] private int lineCount = 0;
    [SerializeField] private int widthInChars = 0;

    int fullLines;
    private string[] lines;

    private ExpanderComponent expander;
    private ExpanderComponent Expander {
        get {
            if (expander == null) {
                expander = GetComponent<ExpanderComponent>();
            }
            return expander;
        }
    }

    public void Start() {
        lines = new string[lineCount];
        Clear();
    }

    public void Clear() {
        fullLines = 0;
        for (var i = 0; i < lineCount; i += 1) {
            lines[i] = "";
        }
        textbox.text = "";
    }

    public IEnumerator TestRoutine() {
        yield return WriteLineRoutine("The combat begins!!");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
            "eiusmod tempor incididunt ut labore et dolore magna aliqua.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("BobHuman dealt 10 damages!");
        yield return WriteLineRoutine("BubHuman deflected by ESP.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("This is a run-on sentence which is composed of multiple sentences in sequence" +
            " strung together in a nonsense fashion. My hope is that this stress the multiline capabilities of the" +
            " text area, and hopefully illustrates that even when the text exceeds one box length, everything is still" +
            " handled properly.");
        yield return WriteLineRoutine("");
        yield return WriteLineRoutine("Now wasn't that fun?");
    }

    public IEnumerator WriteLineRoutine(string line) {
        Global.Instance().Input.PushListener(this);

        var words = line.Split(' ');
        for (var at = 0; at < words.Length; at += 1) {
            int added = 0;
            var firstLine = new StringBuilder();
            var leadingSpace = false;
            while (at < words.Length) {
                string word = words[at];
                int run = word.Length + (leadingSpace ? 1 : 0);
                if (added + run > widthInChars) {
                    break;
                }
                if (leadingSpace) {
                    firstLine.Append(" ");
                }
                firstLine.Append(word);
                added += run;
                at += 1;
                leadingSpace = true;
            }
            
            if (fullLines < lineCount) {
                lines[fullLines] = firstLine.ToString();
                fullLines += 1;
            } else {
                for (var i = 0; i < lineCount - 1; i += 1) {
                    lines[i] = lines[i + 1];
                }
                lines[lineCount - 1] = firstLine.ToString();
            }

            typingStartIndex = 0;
            var fullMessage = new StringBuilder();
            for (var i = 0; i < lineCount; i += 1) {
                if (i < fullLines - 1) {
                    typingStartIndex += lines[i].Length + 2; // +2 for \r\n
                }

                fullMessage.AppendLine(lines[i]);
            }
            
            yield return TypeRoutine(fullMessage.ToString(), false);
        }

        Global.Instance().Input.RemoveListener(this);
    }

    public IEnumerator ShowRoutine() {
        if (Expander != null) {
            Expander.Hide();
            yield return Expander.ShowRoutine();
        }
    }

    public IEnumerator HideRoutine() {
        if (Expander != null) {
            Expander.Show();
            yield return Expander.HideRoutine();
        }
    }
}