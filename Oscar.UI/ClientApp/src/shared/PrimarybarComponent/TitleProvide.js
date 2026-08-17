import React ,{useContext,useReducer,createContext} from 'react';


export const TitleProviderContext= createContext();

export const TitleProvider=(props)=>(
    <TitleProviderContext.Provider value={useReducer(props.reducer,props.initialState)}>
        {props.children}
    </TitleProviderContext.Provider>
)

export const useDataProviderValue=()=>useContext(TitleProviderContext)