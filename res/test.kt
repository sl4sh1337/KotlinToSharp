class MyClass {
    var inner: Int = 1337
}

fun main(args: Array<String>) {
    var str: String = "a" + (2 <= 3) + 4 * 7 + 2 % 10
    var flag: Boolean = true || false xor (str >= "a") && (2 + 2 - 1 > 0)
    var myClass = MyClass()
    myClass.inner = "kjhbkb"
    print(str + flag + myClass.inner)
}
