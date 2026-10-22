# Martian Robots

A .NET 10 console app for the Martian Robots challenge. Robots are placed on a rectangular grid on Mars and given a list of instructions. Each robot reports where it ends up, or `LOST` if it moves off the grid.

## Rules

- The grid starts at `(0, 0)` in the bottom-left corner. North is towards increasing `y`, and East is towards increasing `x`.
- Each robot has a position and an orientation (`North`, `East`, `South`, `West`).
- Instructions are single characters and can be upper or lower case:
  - `L` turns the robot 90° left without moving.
  - `R` turns the robot 90° right without moving.
  - `F` moves the robot one tile forward in the direction it is facing.
- If a robot moves off the grid, it is `LOST`. It leaves a *scent* on the last tile it was on, and any remaining instructions are ignored.
- If a later robot on a scented tile is told to move off the grid, that move is skipped and the robot carries on with its next instruction.
- Robots run one after another. Each robot can use the scents left by earlier robots on the same grid.

## Solution structure

| Project | Purpose |
| --- | --- |
| `MartianRobots.Service` | Console entry point (`Program.cs`) that sets up the sample grid and robots and prints the results. |
| `MartianRobots.Service.Models` | Domain models: `GridSurface` (grid, movement and scents), `GridSurfaceTile` and `MartianRobot`. |
| `MartianRobots.Service.Common` | The `Instructions` and `Orientation` enums, plus extension methods for parsing instructions (`ToEnum`) and describing them (`AsDescription`). |
| `MartianRobots.Service.Tests` | xUnit unit tests for the models and extension methods. |

## Getting started

Prerequisite: the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
# Build the solution
dotnet build Tdl.Challenge.slnx

# Run the console app
dotnet run --project MartianRobots.Service

# Run the unit tests
dotnet test Tdl.Challenge.slnx
```

Outside of the bash commands, all of the above can be achieved via the UI in Visual Studio 2026.

While it runs, the app prints each step and draws the grid after every instruction (`R` = robot, `S` = scent, `.` = empty). It then prints the robot's final position. Press Enter to exit.

## Sample

The grid's top-right corner is `(5, 3)`, so `Program.cs` creates it with `new GridSurface(6, 4)`.
You can manually change the size of the grid to produce different results.

| Start | Instructions | Result |
| --- | --- | --- |
| `1 1 E` | `RFRFRFRF` | `Ending Position: 1 1 East` |
| `3 2 N` | `FRRFLLFFRRFLL` | `Ending Position: 3 3 North LOST` |
| `0 3 W` | `LLFFFLFLFL` | `Ending Position: 2 3 South` |

The third robot only survives because the second robot left a scent at `(3, 3)`.

## Usage

```csharp
var grid = new GridSurface(6, 4);

grid.PlaceRobot(new MartianRobot(Orientation.East, ["r", "f", "r", "f"]), 1, 1);
string result = grid.InstructMartianRobot(); // "Ending Position: 0 0 West"
```

## Tests

The tests in `MartianRobots.Service.Tests` cover:

- **Instruction parsing:** valid input in either case, invalid input, and instruction descriptions.
- **Grid setup:** tile initialisation and boundary checks.
- **Robot placement:** out-of-bounds placement and clearing the previous robot's tile.
- **Turning and movement:** left and right turns from every orientation, and forward moves in every direction.
- **Falling off and scents:** falling off each edge, where the scent is left, scents stopping later robots from falling, and instructions after `LOST` being ignored.
- **Sample run:** the full sample input from start to finish.

## Behaviour notes

- Results show the orientation as a full word (`East`) rather than a single letter (`E`).
- `PlaceRobot` clears tile occupancy before placing a robot, so only one robot is on the grid at a time. Scents are kept between robots.
- A scent applies to the tile, not to a direction. It blocks a move off the grid in any direction from that tile.
- `InstructMartianRobot` updates the `MartianRobot` object's `Orientation` as the instructions run.

## Future Enhancements

- Allow multiple robots to traverse the grid at the same time, with built in collision detection.
- Better visualisation of where the robot is orientated.
- Introduce different robot variants i.e. robots that can detect where the edges are and avoid as necessary.
- Update the Console App to allow configuration of the grid and the robots.
