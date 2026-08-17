import React from "react";
import { useDataProviderValue } from "./TitleProvide";
import EpisodeGrid from "../../Archieve/EpisodeGrid";
import { Link } from "react-router-dom";
import './DetailComponent.css';

function DetailComponent() {
  
  const [{ title }] = useDataProviderValue();
  return (
    <div className="container">
      <div  className="detailcontainer">
        <Link>{title}</Link>
      </div>
      <div className="table" >
          <EpisodeGrid />
       </div>

     
    </div>
  );
}

export default DetailComponent;
