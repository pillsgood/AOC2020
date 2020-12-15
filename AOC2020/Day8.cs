using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Pillsgood.AdventOfCode.Abstractions;
using Pillsgood.AdventOfCode.Core;

namespace AOC2020
{
    public class Day8 : IPuzzle
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(provider => new PuzzleInput<List<Instruction>>(provider, Process).Value);
        }

        private static List<Instruction> Process(string value)
        {
            var instructions = new List<Instruction>();
            foreach (var line in value.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                _ = new Instruction(instructions, line);
            }

            return instructions;
        }

        private delegate void InstructionHandle(ref int index, ref int accumulator, int value);

        private static readonly InstructionHandle Nop =
            (ref int index, ref int accumulator, int value) => { };

        private static readonly InstructionHandle Jmp =
            (ref int index, ref int accumulator, int value) => index += value;

        private static readonly InstructionHandle Acc =
            (ref int index, ref int accumulator, int value) => accumulator += value;

        private class Instruction
        {
            private readonly List<Instruction> _instructions;
            public InstructionHandle instructionHandle;
            public bool Repeat { get; private set; } = false;

            private readonly int _value;

            public Instruction(List<Instruction> instructions, string input)
            {
                _instructions = instructions;
                _value = int.Parse(input[3..].Trim());
                instructionHandle = input[..3] switch
                {
                    "nop" => Nop,
                    "acc" => Acc,
                    "jmp" => Jmp,
                    _ => throw new Exception("failed top parse input")
                };
                instructions.Add(this);
            }

            public Instruction(List<Instruction> instructions, Instruction instruction)
            {
                _instructions = instructions;
                _value = instruction._value;
                Repeat = instruction.Repeat;
                instructionHandle = instruction.instructionHandle;
                instructions.Add(this);
            }

            public Instruction Run(ref int index, ref int accumulator)
            {
                Repeat = true;
                instructionHandle?.Invoke(ref index, ref accumulator, _value);
                if (instructionHandle != Jmp)
                {
                    index++;
                }

                return index < _instructions.Count ? _instructions[index] : null;
            }
        }

        private class State
        {
            public int accumulator;
            private int _index;

            private readonly List<Instruction> _instructions;

            public State(IEnumerable<Instruction> instructions)
            {
                _instructions = new List<Instruction>(instructions);
                accumulator = 0;
                _index = 0;
            }

            private State(State state)
            {
                _instructions = new List<Instruction>();
                foreach (var stateInstruction in state._instructions)
                {
                    _ = new Instruction(_instructions, stateInstruction);
                }

                accumulator = state.accumulator;
                _index = state._index;
            }

            public IEnumerable<State> FindBranchingPoint()
            {
                var instruction = _instructions[_index];
                while (!instruction.Repeat)
                {
                    if (instruction.instructionHandle == Nop || instruction.instructionHandle == Jmp)
                    {
                        var branch = new State(this);
                        branch._instructions[_index].instructionHandle =
                            branch._instructions[_index].instructionHandle == Nop ? Jmp : Nop;
                        yield return branch;
                    }

                    instruction = instruction.Run(ref _index, ref accumulator);
                }
            }

            public bool RunUntilTerminate()
            {
                var instruction = _instructions[_index];

                while (!instruction.Repeat)
                {
                    instruction = instruction.Run(ref _index, ref accumulator);
                    if (_index >= _instructions.Count)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Part(1)]
        private string Part1(List<Instruction> instructions)
        {
            var accumulator = 0;
            var index = 0;
            var instruction = instructions[index];
            while (!instruction.Repeat)
            {
                instruction = instruction.Run(ref index, ref accumulator);
            }

            var answer = accumulator;
            return answer.ToString();
        }

        [Part(2)]
        private string Part2(List<Instruction> instructions)
        {
            var state = new State(instructions);
            var answer = state.FindBranchingPoint().FirstOrDefault(branch =>
                branch.RunUntilTerminate())?.accumulator;
            return answer.ToString();
        }
    }
}