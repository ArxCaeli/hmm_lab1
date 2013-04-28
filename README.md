hmm_lab1
========

hmm forward/backward viterbi algs implementation

Results:
Здравствуйте.
Наконец-то нашел время доделать 1ую лабу.
Вариант 6. (получилось не так однозначно как ожидал)

Результаты:
Если коротко:
При ограничении = 0.7 на forward-backward:
TP = 48 TN = 98 FP = 0 FN = 5
TP - Viterbi и ForwBackw показывают St1, TN - St2
Чувствительность = 48 / 48 + 5 = 0.905660377
Специфичность = 48 / 48 + 0 = 1
Количество отсеянных элементов последовательности 249 (из 402)
Не коротко - ниже.

Исходники:
Делал всё на mono (это тот же C# тоесть та же Java)
(Вы не на этом просили сделать, но надеюсь результатов будет достаточно)

Код лежит в https://github.com/ArxCaeli/hmm_lab1
ForwardBackward:
https://github.com/ArxCaeli/hmm_lab1/blob/master/HmmFramework/ForwardBackward.cs
(методы CalcBackwardState, CalcForwardState)
Тут я поигрался с ExpLog оптимизацией против underflow'a
ничего особо умного: http://www.youtube.com/watch?v=-RVM21Voo7Q
Viterbi:
https://github.com/ArxCaeli/hmm_lab1/blob/master/HmmFramework/Viterbi.cs
(метод MakeStep)


Результаты детально:
Распределение по последовательности в V6Results.xlsx:
General - результаты forward backward, Путь построенный по Viterbi.
На графиках:
красная линия - forward backward
голубая - viterbi (V = 0 - state 2, V = 1 - state 1)

Viterbi - результаты Viterbi
V6vsV9 сравнение работы на разных вариантах входных данных при различных ограничениях

Выводы:
6 вариант сложнее 9го (см V6vsV9 в файле).
в 9ом при минимальных ограничениях получаются отличные результаты (FP = 1 / FN = 1)
в 6 только начиная с ограничения 0.7 FP / FN около 1% и большое отсеивание из последовательности

Чем это вызвано?
C точки зрения системы у нас есть проблемма:
Вероятности эмиссий St1 и St2 очень похожи, как и переходы
=================================
State St1 Transitions:    B = 0.000    St1 = 0.960    St2 = 0.039    E = 0.001
 Emissions    a = 0.139    b = 0.639    c = 0.222
=================================
State St2 Transitions:    B = 0.000    St1 = 0.014    St2 = 0.952    E = 0.034
 Emissions    a = 0.264    b = 0.562    c = 0.174
=================================

Тоесть v6 - не однозначная система, соответсвенно алгоритмы дают не достаточно уверенные выводы.
в V9 эмиссии достаточно хорошо отделяют состояния, что дает хорошие результаты.

