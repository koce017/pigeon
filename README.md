# Pigeon

## Sample: Fibonacci sequence
```c#
let n = prompt_i("n: ");

for i = 1 to n
    print(fibonacci(i));

int fibonacci(int i) {
    if (i == 0 || i == 1)
        return i;
    return fibonacci(i - 1) + fibonacci(i - 2);
}
```

## Data Types
* `int`
* `float`
* `string`
* `bool`
* `[]int`
* `[]float`
* `[]string`
* `[]bool`
* `set`

## Built-In Functions
```c#
int len(string);
string char_get(string, int);
string char_set(string, int, string);

int len([]int);
int len([]float);
int len([]string);
int len([]bool);

void list_add([]int, int);
void list_add([]float, float);
void list_add([]string, string);
void list_add([]bool, bool);

bool set_in(set, int);
bool set_in(set, float);
bool set_in(set, string);
bool set_in(set, bool);

void set_add(set, int);
void set_add(set, float);
void set_add(set, string);
void set_add(set, bool);

void set_remove(set, int);
void set_remove(set, float);
void set_remove(set, string);
void set_remove(set, bool);
```

## Sample: Parsing Tree
![Parsing Tree Example](tree.png)
