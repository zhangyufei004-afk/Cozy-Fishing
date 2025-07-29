# Fishing Game - Code Style Guidelines
## Table of Contents
1. [Naming Conventions](#naming-conventions)
   1. [Variable Names](#variables-names)
   2. [Name Style Rules](#name-style-rules)
2. [Documentation](#documentation)
3. [Name Spaces](#name-spaces)

## Naming Conventions
### Variables Names
Variable names should be informative, specific and descriptive. Single letter variable names are not permitted unless they are used in the context of array iteration. <br>
#### Variable Name Examples:
| Good Variable Names                    | Bad Variable Names | Notes                                                                           |
|----------------------------------------|--------------------|---------------------------------------------------------------------------------|
| `int healthPoints`                     | `int hp`           | Variables names reveal intent. <br>Make names Searchable and <br>Pronounceable. |
| `bool isPlayerDead` <br> `bool isDead` | `bool dead`        | Booleans ask a question that<br>can be answered true or false.                  |
| `int elapsedTimeInDays`                | `int days`         | Be specific about the <br>measurement unit.                                     |
| `int movementSpeed`                    | `int mvmtSpeed`    | Use Nouns.                                                                      |
| `string teamName`                      | `string tName`     |                                                                                 |

### Name Style Rules


| Style           | Type                                                                                                                                                                                                |
|-----------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| UpperCamelCase  | - Structs, Classes and Namespaces <br> - Methods <br> - Properties <br> - Events <br> - Instance Fields (Not Private) <br> - Static Fields (Not Private) <br> - Enum Members <br> - Local Functions |
| IUpperCamelCase | - Interfaces (append with an I to start)                                                                                                                                                            |
| TUpperCamelCase | - Type Parameters                                                                                                                                                                                   |
| LowerCamelCase  | - Local Variables <br> - Local Constants <br> - Parameters<br/> - Unity Serialized Fields                                                                                                           |
| _LowerCamelCase | - Instance Fields (Private) <br/> - Static Fields (Private)                                                                                                                                         |
| ALL_CAPS        | - Constants (Class Level)                                                                                                                                                                           |

#### Name Style Example:
```csharp
namespace ExampleNameSpace 
{
    class TExampleClass<T> 
    {
        const int EXAMPLE_CONST_INT = 10;
        
        public int ExampleProperty 
        {
            get => return _examplePrivateField; 
            set => _examplePrivateField = value;
        }
        
        protected string ExamplePublicProperty;
        private int _examplePrivateField;
        
        public TExampleClass() 
        {   
            // Constructor 
        }
        
        public DoSomethingMethod(int exampleIntParameter) 
        {   
            // Do something in This Method
        }
    }
    
    interface IExampleInterface 
    {
        void ExampleInterfaceMethod();
    }
}
```

## Documentation
>*Code is like humor. If you have to explain it, it’s bad.*
><p>&emsp; – Cory House, software architect and author</p>
### Comments
Most of your code won’t need comments if you follow KISS principles and break your code into
easy-to-digest logical parts. Well-named variables and functions will explain themselves.

<br>Where comments make sense, you should explain the why, not the what. Did you
make specific decisions that are not immediately obvious? Is there a tricky bit of logic that
needs clarification? Useful comments reveal information not gleaned from the code itself.

Here are some Guidelines when writing comments:
- **Don't add comments to replace bad code:** If you need to add a comment to explain a convoluted tangle of logic, restructure 
your code to be more obvious. Then you won't need the comment. 
- **A Properly named class, variable or method serves in place of a comment**: Is the code self-explanatory? Then reduce noise and skip the comment.
- **Place the comment on a separate line where possible**: Keeping the comment on another line provides extra clarity.
- **Use a tooltip instead of a comment for Serialized Fields**: If your fields in the inspector needs explanation, add a tooltip attribute and skip the separate comment. The tooltip will appear in the editor. 
```csharp
// EXAMPLE: Tooltip replaces comment

[Tooltip("The amount of side-to-side friction.")]
[SerializedField] private float Grip;
```
- **Insert one space between the comment delimiter `//` and the comment text.**
- **Remove commented out code:** Though commenting out statements may be normal during testing and development, don't leave commented code lying around. Rely on your source control for previous versions of the code. Then have the courage to delete those lines of code. 
- **Keep your TODO comments up-to-date**: As you complete tasks, make sure you scrub the TODO comments you've left as a reminder. Outdated comments are distractions. Adding a date to the TODO will help keep track of when the TODO was added, and if it is still relevant.
- **Avoid journals**: The comments are not a place for your dev diary. There’s no need to log
  everything you’re doing in a comment when you start a new class. Proper use of source
  control makes this redundant.

### XML Documentation


## Name Spaces

