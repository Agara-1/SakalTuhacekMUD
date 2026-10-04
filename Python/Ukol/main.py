import time
import subprocess
import sys
import os
from collections import Counter


# ukol 1 nepravda - set nejde vzdy, list nejde zahashovat a set nema poradi

try:
    # s = {[1, 2], [3, 4]} nejde zahashovat takze neplati
    l = [[1, 2], [3, 4]]
    print(l)
    print(hash("ahoj"))
    print(hash((1, 2)))
#    print(hash([1, 2]))
    print("prošlo")
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 2 nepravda - to stejny jako 1, do setu nemuzeme vlozit list protoze nebere nehashovatelne prvky

l = []
l.append([1, 2])
print(l)

try:
    s = set()
 #   s.add([1, 2])
    print("prošlo")
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 3 pravda - tuple je list, ktery nejde zmenit

t = (1, 2, 3)

# 1) prepsat prvek
try:
    t[0] = 99
    print("přepsání prošlo")
except Exception as e:
    print("přepsání:", type(e).__name__, e)

# 2) pridat prvek
try:
    t.append(4)
    print("přidání prošlo")
except Exception as e:
    print("přidání:", type(e).__name__, e)

# 3) odebrat prvek
try:
    del t[0]
    print("odebrání prošlo")
except Exception as e:
    print("odebrání:", type(e).__name__, e)


# ukol 4 pravda - index se zmeni, pomoci l.pop(tady je index) odstranime prvek a ostatni se posunou

try:
    l = [4, 5, 6, 8]
    print(l.index(8))
    l.pop(0)
    print(l.index(8))
except Exception as e:
    print("odebrání:", type(e).__name__, e)


# ukol 5 nepravda - dictionary nema indexy ale klice k prvkum a nemuze slicovat jako list

l = ["a", "b", "c", "d"]
d = {0: "a", 1: "b", "uyg": "c"}

print(l[0], d["uyg"])
print("list [1:3]:", l[1:3])
try:
    print("dict [1:3]:", d[1:3])
except Exception as e:
    print("dict [1:3]: chyba", type(e).__name__, e)


# ukol 6 pravda - set nema poradi, porovnava jen jestli ma stejne prvky

a = {1, 2, 3}
b = {3, 1, 2}

print(a)
print(b)
print("a == b:", a == b)


# ukol 7 pravda - set je mnohem rychlejsi protoze hleda pres hash, list jde prvek po prvku

l = list(range(100000))
s = set(l)

hledane = -1

start = time.perf_counter()
for i in range(1000):
    hledane in l
end = time.perf_counter()
print("list:", end - start)

start = time.perf_counter()
for i in range(1000):
    hledane in s
end = time.perf_counter()
print("set:", end - start)


# ukol 8 nepravda - list ma porad poradi a indexy a jde do nej pridat duplicita, set ne

l = [3, 1, 2]
s = {3, 1, 2}

print(l[0])
try:
    print(s[0])
except Exception as e:
    print("chyba:", type(e).__name__, e)

l.append(1)
s.add(1)
print("list:", l)
print("set:", s)


# ukol 9 nepravda - d[0] nehleda prvni prvek ale klic 0

d = {"jablko": 10, "hruska": 20}

try:
    print(d[0])
except Exception as e:
    print("chyba:", type(e).__name__, e)

d2 = {5: "x", 0: "y"}
print(d2[0])            # vypise y, ale prvni je x
print(list(d2)[0])      # takhle se dostanu k prvnimu klici


# ukol 10 nepravda - list musi hledat prvek po prvku (O(n)), set najde hned (O(1))

l = list(range(100000))
s = set(l)

hledane = 99999         # posledni prvek

start = time.perf_counter()
for i in range(1000):
    hledane in l
end = time.perf_counter()
print("list:", end - start)

start = time.perf_counter()
for i in range(1000):
    hledane in s
end = time.perf_counter()
print("set:", end - start)


# ukol 11 pravda - tuple drzi odkaz na list a ten list jde menit

t = (1, [2, 3])
t[1].append(4)
print(t)


# ukol 12 pravda - set kazdou hodnotu ulozi jen jednou takze se pocty ztrati

l = [1, 1, 2, 3, 3, 3]
print(l, len(l))
print(set(l), len(set(l)))
print(Counter(l))


# ukol 13 nepravda - tuple ma porad stejne odkazy (stejne id), zmenil se jen list uvnitr

t = (1, [2, 3])
print(id(t), id(t[1]))
t[1].append(4)
print(id(t), id(t[1]))
print( t)


# ukol 14 nepravda - stejny obsah ale jiny datovy typ

print([1, 2], type([1, 2]))
print((1, 2), type((1, 2)))

a = {1, 2}
b = frozenset({1, 2})
print("a == b:", a == b)
print(type(a), type(b))


# ukol 15 pravda - list umi duplicity, poradi a listy uvnitr, set ne

print([1, 1, 2], set([1, 1, 2]))
print([2, 1], set([2, 1]))

try:
    print(set([[1], [2]]))
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 16 nepravda - klice jsou unikatni, druhy prepise prvni

d = {"a": 1, "a": 2}
print(d, len(d))


# ukol 17 nepravda - poradi v setu zavisi na hashi a ten je u stringu pri kazdem spusteni jiny

kod = "print({'jablko', 'hruska', 'banan', 'kiwi', 'mango'})"

for seed in ["1", "2", "3"]:
    env = dict(os.environ, PYTHONHASHSEED=seed)
    vystup = subprocess.run([sys.executable, "-c", kod], env=env, capture_output=True, text=True)
    print(vystup.stdout.strip())


# ukol 18 nepravda - i kdyz nepridavam ani neodebiram, muzu chtit prvky menit nebo radit

l = [5, 3, 8]
l[0] = 10
l.sort()
print(l)

t = (5, 3, 8)
try:
    t[0] = 10
except Exception as e:
    print("chyba:", type(e).__name__, e)

try:
    t.sort()
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 19 pravda - ztrati se duplicity a muze se zmenit poradi

l = [3, 1, 3, 2]
zpet = list(set(l))
print(l, zpet)
print(l == zpet)


# ukol 20 pravda - bod [3, 7] neni stejny jako bod [7, 3]

bod1 = [3, 7]
bod2 = [7, 3]
print(bod1 == bod2)
print(set(bod1) == set(bod2))


# ukol 21 nepravda - == porovnava jen obsah, ne jestli je to stejny objekt

a = [1, 2]
b = [1, 2]
c = a               # c je jen jine jmeno pro a

print("a == b:", a == b)
print("a is b:", a is b)

a.append(3)
print("c:", c)      # c se zmenilo
print("b:", b)      # b ne


# ukol 22 pravda - is znamena ze je to jeden objekt, jen dve jmena

a = [1]
b = a
print(a is b)
print(hex(id(a)), hex(id(b)))
a.append(2)
print(b)


# ukol 23 pravda - string umi [0], [-1], slicing, ale nejde menit

s = "ahoj"
print(s[0], s[-1], s[1:3], len(s))

try:
    s[0] = "x"
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 24 nepravda - set nema poradi ani indexy

s = {10, 20, 30}
try:
    print(s[1])
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 25 pravda - dictionary ma klic (identifikator) a hodnotu

ceny = {"chleba": 35, "mleko": 22}
for klic, hodnota in ceny.items():
    print(klic, hodnota)


# ukol 26 pravda - [[]] * 10 udela 10 odkazu na jeden list

l = [[]] * 10
l[0].append(1)
print(l)
print(id(l[0]), id(l[5]))


# ukol 27 pravda - set uz tu hodnotu ma, takze se nic nezmeni

s = set()
s.add(5)
kopie = set(s)
s.add(5)
print(kopie, s)
print(s == kopie)


# ukol 28 pravda - set si pamatuje jen ze tam hodnota je, ne kolikrat

l = ["a", "b", "a", "a"]
print(l.count("a"))
print(set(l))


# ukol 29 pravda - tuple jde zahashovat, ale jen kdyz v nem neni list

mesta = {(50.08, 14.42): "Praha"}
print(mesta[(50.08, 14.42)])

  # d = {(1, [2]): "x"}



# ukol 30 nepravda - tuple je nemenny ale list v nem menit jde (jako 11)

t = (1, [2])
t[1].append(3)
print(t)


# ukol 31 pravda - po zmene jednoho uz nejsou stejne

a = [1, 2]
b = [1, 2]
print(a == b)
a.append(3)
a[0] = 5
print(a)
print(a == b)


# ukol 32 pravda - kdyz se pocet nezmeni, vsechny prvky jsou unikatni

l = [4, 7, 1]
print(len(l) == len(set(l)))

l2 = [4, 7, 4]
print(len(l2) == len(set(l2)))


# ukol 33 nepravda - unikatni musi byt klice, hodnoty se opakovat muzou

d = {"a": 1, "b": 1}
print(d, len(d))


# ukol 34 nepravda - zalezi na tom co s daty delam, ne kolik jich je

nakup = ["rohlik", "rohlik", "mleko"]   # potrebuju pocet kusu -> list
ruzne = set(nakup)                      # jen co koupit -> set
ceny = {"rohlik": 3, "mleko": 22}       # hledani podle jmena -> dict
print(nakup)
print(ruzne)
print(ceny["mleko"])


# ukol 35 nepravda - staci jeden dict (Counter), in je rychly a pocty se zachovaji

c = Counter(["a", "b", "a", "a"])
print("a" in c)
print(c["a"])


# ukol 36 pravda - trasa tam je jina nez trasa zpatky

trasa = ["Praha", "Brno", "Ostrava"]
zpatky = ["Ostrava", "Brno", "Praha"]
print(trasa == zpatky)
print(set(trasa) == set(zpatky))


# ukol 37 pravda - oba umi indexovat, ale tuple nejde menit

l = [1, 2]
t = (1, 2)
print(l[0], t[0])

l[0] = 9
print(l)

try:
    t[0] = 9
except Exception as e:
    print("chyba:", type(e).__name__, e)


# ukol 38 pravda - list ma navic poradi a duplicity

l = [2, 1, 1]
s = {1, 2}
print(set(l) == s)
print(l)


# ukol 39 pravda - in funguje u obou

print(2 in [1, 2])
print(2 in {1, 2})


# ukol 40 nepravda - jedno mereni nic nedokazuje, u maleho listu muze byt list stejne rychly

l = [0, 1, 2, 3, 4]
s = set(l)

for pokus in range(3):
    start = time.perf_counter()
    for i in range(100000):
        0 in l
    end = time.perf_counter()
    print("list:", end - start)

    start = time.perf_counter()
    for i in range(100000):
        0 in s
    end = time.perf_counter()
    print("set:", end - start)


# ukol 41 pravda - cim vic prvku, tim vic je list pomalejsi nez set

for n in [100, 10000, 1000000]:
    a = list(range(n))
    b = set(a)

    start = time.perf_counter()
    for i in range(50):
        -1 in a
    cas_list = time.perf_counter() - start

    start = time.perf_counter()
    for i in range(50):
        -1 in b
    cas_set = time.perf_counter() - start

    print(n, "list je pomalejsi", round(cas_list / cas_set), "krat")


# ukol 42 nepravda - pristup pres index je porad stejne rychly, zdvojnasobi se jen in

for n in [1000000, 2000000]:
    l = list(range(n))

    start = time.perf_counter()
    for i in range(10000):
        l[n // 2]
    end = time.perf_counter()
    print(n, "index:", end - start)

    start = time.perf_counter()
    for i in range(20):
        -1 in l
    end = time.perf_counter()
    print(n, "in:", end - start)


# ukol 43 pravda - v obou pripadech se projde cely list

l = list(range(1000000))

start = time.perf_counter()
for i in range(20):
    999999 in l
end = time.perf_counter()
print("posledni:", end - start)

start = time.perf_counter()
for i in range(20):
    -1 in l
end = time.perf_counter()
print("chybejici:", end - start)


# ukol 44 nepravda - index je O(1) (porad stejne rychly), in je O(n) (zpomaluje se)

for n in [10000, 1000000]:
    l = list(range(n))

    start = time.perf_counter()
    for i in range(20):
        l[n - 1]
    end = time.perf_counter()
    print(n, "index:", end - start)

    start = time.perf_counter()
    for i in range(20):
        (n - 1) in l
    end = time.perf_counter()
    print(n, "in:", end - start)


# ukol 45 nepravda - kazda kolekce umi neco co list ne

print({"chleba": 35}["chleba"])     # dict - hledani podle klice
print(set([1, 1, 2]))               # set - bez duplicit, rychly in
print({(1, 2): "bod"})              # tuple - muze byt klic


    # d = {[1, 2]: "bod"}             list klicem byt nemuze
