# Fishing Game - Code Style Guidelines
#### Copyright Madison Reilly 2025
## Table of Contents
1. [Naming Conventions](#naming-conventions)
   1. [Variable Names](#variables-names)
   2. [Name Style Rules](#name-style-rules)
2. [Documentation](#documentation)
   1. [Comments](#comments)
   2. [XML Documentation](#xml-documentation)
3. [Name Spaces](#name-spaces)
4. [Class Formatting](#class-formatting)
5. [Further Guidance](#further-guidance)

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
| lowerCamelCase  | - Local Variables <br> - Local Constants <br> - Parameters<br/> - Unity Serialized Fields                                                                                                           |
| _lowerCamelCase | - Instance Fields (Private) <br/> - Static Fields (Private)                                                                                                                                         |
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
All classes and <u>non unity</u> methods should include XML Documentation. This documentation serves as a summary of your classes
and methods to other developers. An example has been provided:
```csharp
namespace XMLExample
{
    /// <summary>
    /// This is an example class. This summary serves to summarise the purpose of the class, and the methods contained in it. 
    /// </summary>
    class XMLExampleClass
    {
        int exampleVariable;
        
        /// <summary>
        /// This is a summary of the Method. It is a high level indicator of what the method does. 
        /// </summary>
        /// <param name="Parameter">This parameter is a string argument. In proper documentation I would describe what the variable is. </param>
        /// <returns>Describe what the function returns specifically here. Does it return a formatted string? Maybe it returns the string mutated? </returns>
        public string ExampleMethod(string Parameter)
        {
            // Do Something
        }
        
        void Start()
        {
            // This is a unity method, so we dont need XML Documentation  
        }
    }
}
```
For more information on XML Documentation and the tags for it, check out the [Microsoft Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags).

## Name Spaces
All classes, structs and enums need to be surrounded in a namespace. This namespace should branch off the root namespace and reflect the subfolder location. 
### For Example:
Let's say you are writing a class named PlayerMovement. This is in the `Assets\Scripts\Player` folder, and your root namespace is `FishingGame`. You would then declare your class as follows: 

```csharp
// NOTE: The .Player is important here, it reflects the subfolder to 'scripts'
namespace FishingGame.Player
{
    /// <summary>
    /// Responsible for PlayerMovement in the Fishing Game. 
    /// Moves the player foward, back, left, and right. Also allows for sprinting, and walking. 
    /// </summary>
    class PlayerMovement : MonoBehaviour
    {
        // The class is implemented here
    }
}
```

The root namespace for each project is declared in the Unity Project Settings.  

## Class Formatting
All Classes and Structs should follow an identical structure. This ensures other developers know the location of your fields, properties, public and private methods.
<br>Classes/Structs should be ordered from top to bottom as follows:
1. Properties
2. Fields
3. Events/Delegates
4. Monobehaviour Methods (Unity Methods)
5. Public Methods
6. Private/Internal Methods

## Further Guidance
For any further questions in regard to this document, please contact [Madison Reilly](mailto:jre129@uclive.ac.nz).