class MyClass {
    var inner: String = "aaaaaa"
}

fun main(args: Array<String>) {
    var str: String = "a"
    var flag: Boolean = true
    var myClass: MyClass = MyClass()
    do {
        str = str + "a";
        flag = !flag
    } while (str < myClass.inner);
    print(flag)
}
