import { useSelector } from 'react-redux';
import ModalCreateInvoiceFEFO from './ModalCreateInvoiceFEFO';
import ModalCreateInvoiceLot from './ModalCreateInvoiceLot';

const ModalCreateInvoice = (props) => {
    const config = useSelector(state => state.user.config);

    if (config === 'fefo') {
        return <ModalCreateInvoiceFEFO {...props} />;
    }
    return <ModalCreateInvoiceLot {...props} />;
};

export default ModalCreateInvoice;
