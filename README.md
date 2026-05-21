# README.md

````markdown
# Лабораторная работа №7  
## Анализ и преобразование кода с использованием Clang и LLVM

**Выполнил:** Пузырный Д.А.  
**Группа:** АП-327  

---

# Цель работы

Познакомиться с инструментами Clang и LLVM, научиться:

- получать AST (Abstract Syntax Tree);
- генерировать LLVM IR;
- применять оптимизации;
- строить CFG (Control Flow Graph);
- анализировать промежуточное представление программы.

---

# Используемое ПО

Работа выполнялась в Ubuntu (Oracle VirtualBox).

## Установка инструментов

```bash
sudo apt update
sudo apt install -y clang llvm llvm-dev llvm-runtime graphviz xdg-utils
````

## Проверка версий

```bash
clang --version
opt --version
dot -V
llvm-config --version
```

---

# Структура проекта

```text
lab7/
├── main.c
├── notation.c
├── ast_main.txt
├── ast_notation.txt
├── main_00.ll
├── main_02.ll
├── notation_00.ll
├── notation_02.ll
├── cfg_main_00.png
├── cfg_main_02.png
├── cfg_square_00.png
├── cfg_square_02.png
├── cfg_notation_00.png
├── cfg_notation_02.png
└── README.md
```

---

# Основная программа

## Исходный код `main.c`

```c
#include <stdio.h>

int square(int x) {
    return x * x;
}

int main() {
    int a = 5;
    int b = square(a);
    printf("%d\n", b);
    return 0;
}
```

---

# Построение AST

## Генерация AST

```bash
clang -Xclang -ast-dump -fsyntax-only main.c > ast_main.txt
```

## Просмотр AST функций

```bash
grep -A25 "FunctionDecl.*square" ast_main.txt
grep -A45 "FunctionDecl.*main" ast_main.txt
```

## Результат

* `FunctionDecl` — объявление функции;
* `ParmVarDecl` — параметр функции;
* `BinaryOperator` — операция `x * x`;
* `CallExpr` — вызов функции.

---

# Генерация LLVM IR

## Без оптимизации

```bash
clang -O0 -S -emit-llvm main.c -o main_00.ll
```

## С оптимизацией `-O2`

```bash
clang -O2 -S -emit-llvm main.c -o main_02.ll
```

## Просмотр ключевых инструкций

```bash
grep -n "define\|alloca\|load\|store\|call" main_00.ll
grep -n "define\|alloca\|load\|store\|call\|printf" main_02.ll
```

---

# Анализ оптимизации

## Без оптимизации (`-O0`)

В IR присутствуют:

* `alloca`
* `store`
* `load`

Локальные переменные размещаются в памяти.

## После `-O2`

Оптимизации:

* удалены лишние `alloca/store/load`;
* функция `square` встроена;
* выражение `square(5)` вычислено заранее;
* в `printf` передается готовое значение `25`.

---

# Построение CFG

## Создание CFG для `main.c`

```bash
opt -passes=dot-cfg -disable-output main_00.ll
dot -Tpng .main.dot -o cfg_main_00.png
dot -Tpng .square.dot -o cfg_square_00.png
```

## CFG после оптимизации

```bash
opt -passes=dot-cfg -disable-output main_02.ll
dot -Tpng .main.dot -o cfg_main_02.png
dot -Tpng .square.dot -o cfg_square_02.png
```

## Результат

Так как программа не содержит:

* циклов;
* условных операторов;

CFG состоит из одного базового блока.

---

# Индивидуальное задание

## Вариант 2.6 — научная нотация

## Исходный код `notation.c`

```c
#include <stdio.h>
#include <stdlib.h>

int main() {
    const char* str = "3.14e2";
    double value = strtod(str, NULL);
    printf("%f\n", value);
    return 0;
}
```

---

# AST для `notation.c`

```bash
clang -Xclang -ast-dump -fsyntax-only notation.c > ast_notation.txt
```

## Просмотр AST

```bash
grep -A60 "FunctionDecl.*main" ast_notation.txt
```

## Особенности AST

* `"3.14e2"` представлен как `StringLiteral`;
* `strtod` представлен как `CallExpr`.

---

# LLVM IR для `notation.c`

## Генерация IR

```bash
clang -O0 -S -emit-llvm notation.c -o notation_00.ll
clang -O2 -S -emit-llvm notation.c -o notation_02.ll
```

## Анализ инструкций

```bash
grep -n "define\|alloca\|store\|load\|strtod\|printf" notation_00.ll
grep -n "define\|alloca\|store\|load\|strtod\|printf" notation_02.ll
```

---

# Анализ оптимизации `notation.c`

После `-O2`:

* удаляются лишние операции памяти;
* `strtod` остается вызовом функции.

## Причина

LLVM не сворачивает:

```c
strtod("3.14e2", NULL)
```

в константу `314.0`, поскольку:

* функция библиотечная;
* поведение зависит от локали;
* возможна обработка ошибок.

---

# CFG для `notation.c`

## Построение CFG

```bash
opt -passes=dot-cfg -disable-output notation_00.ll
dot -Tpng .main.dot -o cfg_notation_00.png
```

## После оптимизации

```bash
opt -passes=dot-cfg -disable-output notation_02.ll
dot -Tpng .main.dot -o cfg_notation_02.png
```

## Результат

CFG содержит один базовый блок, так как отсутствуют:

* ветвления;
* циклы.

---

# Выводы

В ходе лабораторной работы были изучены:

* Clang;
* LLVM IR;
* AST;
* CFG;
* оптимизации LLVM.

Были получены навыки:

* генерации AST;
* создания LLVM IR;
* применения оптимизаций;
* построения графов потока управления;
* анализа промежуточного представления программы.

Оптимизация `-O2` значительно упрощает IR:

* удаляет лишние операции памяти;
* выполняет inlining;
* сворачивает константы;
* упрощает поток данных.

---

# Контрольные вопросы

## 1. Что такое Clang?

Clang — фронтенд LLVM для языков C/C++.

## 2. Что такое LLVM?

LLVM — инфраструктура компиляции и оптимизации программ.

## 3. Разница между AST и LLVM IR

* AST сохраняет структуру языка;
* LLVM IR — низкоуровневое SSA-представление.

## 4. Зачем нужен IR?

Для независимости от языка и архитектуры.

## 5. Что делает `alloca`?

Выделяет память на стеке.

## 6. Зачем нужна оптимизация?

Для повышения производительности и уменьшения размера кода.

## 7. Что такое SSA?

Форма, где каждая переменная присваивается один раз.

## 8. Что такое CFG?

Граф потока управления программы.

## 9. Арифметика в LLVM IR

```llvm
%1 = add i32 %a, %b
%2 = mul i32 %a, %b
```

## 10. Почему функции удобны для оптимизации?

Они являются независимыми единицами анализа.

## 11. Что происходит с маленькими функциями?

Они могут быть встроены (inline).

## 12. Почему IR удобен для анализа?

IR проще исходного кода и не содержит синтаксического сахара.

---

# Использованные команды

```bash
sudo apt update
sudo apt install -y clang llvm llvm-dev llvm-runtime graphviz xdg-utils

clang --version
opt --version
dot -V
llvm-config --version

mkdir -p ~/lab7
cd ~/lab7

clang -Xclang -ast-dump -fsyntax-only main.c > ast_main.txt
clang -O0 -S -emit-llvm main.c -o main_00.ll
clang -O2 -S -emit-llvm main.c -o main_02.ll

opt -passes=dot-cfg -disable-output main_00.ll
opt -passes=dot-cfg -disable-output main_02.ll

clang -Xclang -ast-dump -fsyntax-only notation.c > ast_notation.txt
clang -O0 -S -emit-llvm notation.c -o notation_00.ll
clang -O2 -S -emit-llvm notation.c -o notation_02.ll

diff notation_00.ll notation_02.ll

opt -passes=dot-cfg -disable-output notation_00.ll
opt -passes=dot-cfg -disable-output notation_02.ll
```

```
```
