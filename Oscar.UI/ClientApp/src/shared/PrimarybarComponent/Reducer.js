export const initialState={
    title:'',
    ID:''
}

const reducer = (state,action)=>{
    console.log('testingdata',action)
    switch(action.type){
        case 'TITLE_CHANGE':
            return{
                ...state,
                title:action.title
            }
            case 'ID_CHANGE':
            return{
                ...state,
                ID:action.ID
            }
        default:
            return state;
    }
}

export default reducer;