import { useSelector } from 'react-redux';
import ModalUpdateInvoiceFEFO from './ModalUpdateInvoiceFEFO';
import ModalUpdateInvoiceLot from './ModalUpdateInvoiceLot';

const ModalUpdateInvoice = (props) => {
    const config = useSelector(state => state.user.config);

    if (config === 'fefo') {
        return <ModalUpdateInvoiceFEFO {...props} />;
    }
    return <ModalUpdateInvoiceLot {...props} />;
};

export default ModalUpdateInvoice;
