class MyClass {
    var inner: Int = 1337
}

fun main(args: Array<String>) {
    var str: String = "a" + (2 <= 3) + 4 * 7 + 2 % 10
    var flag: Boolean = true || false xor (str >= "a") && (2 + 2 - 1 > 0)
    var myClass: MyClass = MyClass()
    myClass.inner = 8 + 12 * 12 % 3 + 1
    print(str + flag + myClass.inner)
}
