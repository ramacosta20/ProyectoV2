

contract smart_contract {
    mapping (address => uint) meta; //guarda la meta de cada usuario    
    mapping (address => uint) dineroRecaudado; //guarda cuanto dinero lleva recaudado cada usuario
    mapping (address => uint) dineroRetirable; //guarda cuanto dinero puede retirar cada usuario

    /* toma como parametro la Address de la persona a la que quiero donar y le asigna una cantidad de fondos mandada
    en el value de la transaccion*/
    function donate (address direcDestinatario) payable returns (bool){
        dineroRecaudado[direcDestinatario] += msg.value;
        if (dineroRecaudado[direcDestinatario] >= meta[direcDestinatario]) {
            dineroRetirable[direcDestinatario] = dineroRecaudado[direcDestinatario];
            return true;
        } else {
            return false
            ;
        }
    }

    // deposita los fondos recaudados por una Address cuando recaudo mas dinero que su meta
    function withdraw (){
        uint cantidad = dineroRetirable[msg.sender];
        // Remember to zero the pending refund before
        // sending to prevent re-entrancy attacks
        dineroRecaudado[msg.sender] = 0;
        dineroRetirable[msg.sender] = 0;
        msg.sender.transfer(cantidad); //manda en ether
    }
}