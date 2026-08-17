import { toast } from 'react-toastify';

export const toastr = (type, message) =>{

  const options = {
    position: "top-right",
    autoClose: 10000,
    hideProgressBar: true,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
    theme: "colored"
  }

  switch(type) {
    case 'error':
      toast.error(message, options);
      break;
    case 'success':
      toast.success(message, options);
      break;
    default:
      toast.info(message, options);
  }
    
}
