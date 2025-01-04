contract Factory {
    mapping(address => address[]) public created;
    
    function createContract() public returns (address) {
        address newContract = address(new ChildContract());
        created[msg.sender].push(newContract);
        return newContract;
    }
}
