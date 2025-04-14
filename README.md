# Pigeon

## Fibonacci Sequence
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
* `list<int>`
* `list<float>`
* `list<string>`
* `list<bool>`
* `set`

## Built-In Functions
```c#
int len(string);
string char_get(string, int);
string char_set(string, int, string);

int len(list<int>);
int len(list<float>);
int len(list<string>);
int len(list<bool>);

void list_add(list<int>, int);
void list_add(list<float>, float);
void list_add(list<string>, string);
void list_add(list<bool>, bool);

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

If functions returns `any`, variants with `_i` (int), `_f` (float), and `_b` (bool) suffixes are available.

## Parsing Trees
Toggle parsing trees with the `$tree` command.

![Parsing Tree Example](tree.png)
